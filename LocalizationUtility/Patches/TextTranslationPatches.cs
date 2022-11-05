using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LocalizationUtility
{
    [HarmonyPatch]
    public static class TextTranslationPatches
    {
        //Maybe instead of storing TextTranslation.TranslationTable_XML store, which work more like dictionaries TextTranslation.TranslationTable
        //private static Dictionary<TextTranslation.Language, TextTranslation.TranslationTable_XML> translationTables = new Dictionary<TextTranslation.Language, TextTranslation.TranslationTable_XML>();


        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.SetLanguage))]
        public static bool TextTranslation_SetLanguage_Prefix(ref TextTranslation.Language lang, TextTranslation __instance)
        {
            if (LocalizationUtility.IsVanillaLanguage(lang)) return true; //We change through TextTranslation_SetLanguage custom languages
            //And vanilla languages with TextTranslation_SetLanguage_Postfix


            if (!LocalizationUtility.Instance.TryGetLanguage(lang, out var language))
            {
                LocalizationUtility.WriteError($"The language {language} doesn't have a translation table");
                lang = TextTranslation.Language.UNKNOWN;
                return true;
            }

            __instance.m_language = language.Language;
            LocalizationUtility.WriteLine($"Loading translation for {language.Name}");
            if (__instance.m_table == null) 
            {
                __instance.m_table.theTable = new Dictionary<string, string>();
            }

            //Now not only we can edit vanila translation tables, but multiple mods can edit the same language table, as long as the keys are unique

            foreach (var pair in language.TranslationTable.table)
                __instance.m_table.theTable[pair.key] = pair.value;

            foreach (var pair in language.TranslationTable.table_shipLog)
                __instance.m_table.theShipLogTable[pair.key] = pair.value;

            foreach (var pair in language.TranslationTable.table_ui)
                __instance.m_table.theUITable[pair.key] = pair.value;

            var onLanguageChanged = (MulticastDelegate)__instance.GetType().GetField("OnLanguageChanged", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
            if (onLanguageChanged != null)
            {
                onLanguageChanged.DynamicInvoke();
            }
            return false;
        }

        //Maybe call this when OnLanguageChanged gets called, but could cause issues, I really don't know
        //A *transpiler* could help to call this right after the xml is loaded (Xen would kill me)
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.SetLanguage))]
        public static void TextTranslation_SetLanguage_Postfix(ref TextTranslation.Language lang, TextTranslation __instance)
        {
            if (lang == TextTranslation.Language.UNKNOWN ||
                !LocalizationUtility.IsVanillaLanguage(lang) ||
                !LocalizationUtility.Instance.TryGetLanguage(lang, out var language))
            {
                return;
            }

            LocalizationUtility.WriteLine($"Loading aditional translations for {language.Name}");

            foreach (var pair in language.TranslationTable.table)
                __instance.m_table.theTable[pair.key] = pair.value;

            foreach (var pair in language.TranslationTable.table_shipLog)
                __instance.m_table.theShipLogTable[pair.key] = pair.value;

            foreach (var pair in language.TranslationTable.table_ui)
                __instance.m_table.theUITable[pair.key] = pair.value;

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

            string text = __instance.m_table.Get(key);
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
