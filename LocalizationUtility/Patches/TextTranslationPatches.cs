using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LocalizationUtility
{
    [HarmonyPatch]
    public static class TextTranslationPatches
    {
        /*This is what the transpiler in TextTranslation_SetLanguage_Transpiler is doing
         
        - (A) Add a check for custom languages in         
                ...
	    this.m_language = lang;
	    this.m_table = null;
        if(!LocalizationUtility.IsVanillaLanguage(lang) && LocalizationUtility.Instance.TryGetLanguage(lang, out var language)) goto TABLE_EDITOR
                ...
         
        - (B) Add table editor        
                             ...
             xmlNode4.SelectSingleNode("value").InnerText));
	      }
	      this.m_table = new TextTranslation.TranslationTable(translationTable_XML);

          TABLE_EDITOR:  LoadLanguageTables(lang, this);

	      Resources.UnloadAsset(textAsset);
                             ...         

        * To do (B)
         1 - Find                 
            IL_0230: ldarg.0
            IL_0231: ldloc.3
            IL_0232: newobj instance void TextTranslation/TranslationTable::.ctor(class TextTranslation/TranslationTable_XML)
            IL_0237: stfld class TextTranslation/TranslationTable TextTranslation::m_table         
        
         2 - Add table editor after match and create label labelToTableEditing to ldarg.1
            ldarg.1 
            ldarg.0
            EmitDelegate for LoadLanguageTables(lang, instance);

        * To do (A)
         1 - Find        
            IL_0011: ldarg.0
            IL_0012: ldarg.1
            IL_0013: stfld valuetype TextTranslation/Language TextTranslation::m_language

            IL_0018: ldarg.0
            IL_0019: ldnull
            IL_001A: stfld     class TextTranslation/TranslationTable TextTranslation::m_table
         
         2 - Add a branch right after match
            ldarg.1
            EmitDelegate for  !LocalizationUtility.IsVanillaLanguage(lang) && LocalizationUtility.Instance.TryGetLanguage(lang, out var language);
            br.true labelToTableEditing (will skip to the label labelToTableEditing if the result on the delegate is true)
        */
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.SetLanguage))]
        public static IEnumerable<CodeInstruction> TextTranslation_SetLanguage_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                return new CodeMatcher(instructions, generator)
                .MatchForward(true, //This searches for part (B) ---------------------------
                   new CodeMatch(OpCodes.Ldarg_0),
                   new CodeMatch(OpCodes.Ldloc_3),
                   new CodeMatch(OpCodes.Newobj),
                   new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == "m_table")
                ).ThrowIfInvalid("Match for skiping method failed") //----------------------
                .Advance(1) //This adds part (B) and creates a label for (A) -------------------------------------
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_1),
                   new CodeInstruction(OpCodes.Ldarg_0),
                   Transpilers.EmitDelegate<Action<TextTranslation.Language, TextTranslation>>((lang, instance) =>
                   {
                       LoadLanguageTables(lang, instance);
                   })
                 )
                .Advance(-3)
                .CreateLabel(out Label labelToTableEditing) //Creates label to ldarg_1
                //------------------------------------------------------------------------------------------------
                .Start().MatchForward(true,//This searches for part (A) ---------------------------
                   new CodeMatch(OpCodes.Ldarg_0),
                   new CodeMatch(OpCodes.Ldarg_1),
                   new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == "m_language"),

                   new CodeMatch(OpCodes.Ldarg_0),
                   new CodeMatch(OpCodes.Ldnull),
                   new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == "m_table")
                 ).ThrowIfInvalid("Match for editing translation table failed")//--------------------
                 .Advance(1).InsertAndAdvance( //This adds part (A) ---------------------------------
                   new CodeInstruction(OpCodes.Ldarg_1),
                   Transpilers.EmitDelegate<Func<TextTranslation.Language, bool>>((lang) =>
                   {
                       return !LocalizationUtility.IsVanillaLanguage(lang) && LocalizationUtility.Instance.TryGetLanguage(lang, out var language);
                   }),
                   new CodeInstruction(OpCodes.Brtrue, labelToTableEditing) //Skips only if condition is true
                   )//-------------------------------------------------------------------------------
                .InstructionEnumeration();
            }
            catch (Exception ex)
            {
                LocalizationUtility.WriteError($"Error at transpiler: msg: {ex.Message} source: {ex.Source} stack tree: {ex.StackTrace}");
                return instructions;
            }
        }

        public static void LoadLanguageTables(TextTranslation.Language lang, TextTranslation instance)
        {
            if (LocalizationUtility.Instance.TryGetLanguage(lang, out var language))
            {
                LocalizationUtility.WriteLine($"Loading translations for {language.Name}");
                if (instance.m_table == null)
                {
                    instance.m_table = new TextTranslation.TranslationTable(language.TranslationTable);
                    return;
                }

                foreach (var pair in language.TranslationTable.table)
                    instance.m_table.theTable[pair.key] = pair.value;

                foreach (var pair in language.TranslationTable.table_shipLog)
                    instance.m_table.theShipLogTable[pair.key] = pair.value;

                foreach (var pair in language.TranslationTable.table_ui)
                    instance.m_table.theUITable[pair.key] = pair.value;
            }
        }     

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation._Translate))]
        private static bool _Translate(
            string key,
            TextTranslation __instance,
            ref string __result)
        {
            if (LocalizationUtility.IsVanillaLanguage(__instance.m_language)) return true;

            if (__instance.m_table == null)
            {
                LocalizationUtility.WriteError("TextTranslation not initialized");
                __result = key;
                return false;
            }

			// Whitespace issues are really annoying so we try to be more forgiving
            string text = __instance.m_table.Get(key) ?? __instance.m_table.Get(key.Trim());
            if (text == null)
            {
                LocalizationUtility.WriteError($"String \"{key}\" not found in table for language {LocalizationUtility.Instance.GetLanguage(__instance.m_language).Name}");
                __result = key;
                return false;
            }

            text = text.Replace("\\\\n", "\n");

            __result = text;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation._Translate_ShipLog))]
        private static bool _Translate_ShipLog(
            string key,
            TextTranslation __instance,
            ref string __result)
        {
            if (LocalizationUtility.IsVanillaLanguage(__instance.m_language)) return true;

            if (__instance.m_table == null)
            {
                LocalizationUtility.WriteError("TextTranslation not initialized");
                __result = key;
                return false;
            }

            string text = __instance.m_table.GetShipLog(key);
            if (text == null)
            {
                LocalizationUtility.WriteError($"String \"{key}\" not found in ShipLog table for language {LocalizationUtility.Instance.GetLanguage(__instance.m_language).Name}");
                __result = key;
                return false;
            }

            text = text.Replace("\\\\n", "\n");

            __result = text;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation._Translate_UI))]
        private static bool _Translate_UI(
            int key,
            TextTranslation __instance,
            ref string __result)
        {
            if (LocalizationUtility.IsVanillaLanguage(__instance.m_language)) return true;

            if (__instance.m_table == null)
            {
                LocalizationUtility.WriteError("TextTranslation not initialized");
                __result = key.ToString();
                return false;
            }

            string text = __instance.m_table.Get_UI(key);
            if (text == null)
            {
                LocalizationUtility.WriteError($"UI String #{key} not found in table for language {LocalizationUtility.Instance.GetLanguage(__instance.m_language).Name}");
                __result = key.ToString();
                return false;
            }

            text = text.Replace("\\\\n", "\n");

            __result = text;

            return false;
        }
    }
}
