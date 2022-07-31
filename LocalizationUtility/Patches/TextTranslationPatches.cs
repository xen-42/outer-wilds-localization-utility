using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace LocalizationUtility
{
    [HarmonyPatch]
    public static class TextTranslationPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.SetLanguage))]
        public static bool TextTranslation_SetLanguage(ref TextTranslation.Language lang, TextTranslation __instance)
        {
            if (LocalizationUtility.IsVanillaLanguage(lang)) return true;

            var language = LocalizationUtility.Instance.GetLanguage(lang);

            if (language == null)
            {
                lang = TextTranslation.Language.UNKNOWN;
                return true;
            }

            __instance.m_language = language.Language;
            __instance.m_table = null;

            var path = language.TranslationPath;

            LocalizationUtility.WriteLine($"Loading translation from {path}");

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(ReadAndRemoveByteOrderMarkFromPath(path));

                var translationTableNode = xmlDoc.SelectSingleNode("TranslationTable_XML");

                if (translationTableNode == null)
                {
                    LocalizationUtility.WriteError($"TranslationTable_XML could not be found in translation file for language {language.Name}");
                    lang = TextTranslation.Language.UNKNOWN;
                    return true;
                }

                var translationTable_XML = new TextTranslation.TranslationTable_XML();

                // Add regular text to the table
                foreach (XmlNode node in translationTableNode.SelectNodes("entry"))
                {
                    var key = node.SelectSingleNode("key").InnerText;
                    var value = node.SelectSingleNode("value").InnerText;

                    if (language.Fixer != null) value = language.Fixer(value);

                    translationTable_XML.table.Add(new TextTranslation.TranslationTableEntry(key, value));
                }

                // Add ship log entries
                foreach (XmlNode node in translationTableNode.SelectSingleNode("table_shipLog").SelectNodes("TranslationTableEntry"))
                {
                    var key = node.SelectSingleNode("key").InnerText;
                    var value = node.SelectSingleNode("value").InnerText;

                    if (language.Fixer != null) value = language.Fixer(value);

                    translationTable_XML.table_shipLog.Add(new TextTranslation.TranslationTableEntry(key, value));
                }

                // Add UI
                foreach (XmlNode node in translationTableNode.SelectSingleNode("table_ui").SelectNodes("TranslationTableEntryUI"))
                {
                    // Keys for UI are all ints
                    var key = int.Parse(node.SelectSingleNode("key").InnerText);
                    var value = node.SelectSingleNode("value").InnerText;

                    if (language.Fixer != null) value = language.Fixer(value);

                    translationTable_XML.table_ui.Add(new TextTranslation.TranslationTableEntryUI(key, value));
                }

                __instance.m_table = new TextTranslation.TranslationTable(translationTable_XML);

                // Goofy stuff to envoke event
                var onLanguageChanged = (MulticastDelegate)__instance.GetType().GetField("OnLanguageChanged", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
                if (onLanguageChanged != null)
                {
                    onLanguageChanged.DynamicInvoke();
                }
            }
            catch (Exception e)
            {
                LocalizationUtility.WriteError($"Couldn't load translation for language {language.Name}: {e.Message}{e.StackTrace}");
                lang = TextTranslation.Language.UNKNOWN;
                return true;
            }

            return false;
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

        public static string ReadAndRemoveByteOrderMarkFromPath(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            byte[] preamble1 = Encoding.UTF8.GetPreamble();
            byte[] preamble2 = Encoding.Unicode.GetPreamble();
            byte[] preamble3 = Encoding.BigEndianUnicode.GetPreamble();
            if (bytes.StartsWith(preamble1))
                return Encoding.UTF8.GetString(bytes, preamble1.Length, bytes.Length - preamble1.Length);
            if (bytes.StartsWith(preamble2))
                return Encoding.Unicode.GetString(bytes, preamble2.Length, bytes.Length - preamble2.Length);
            return bytes.StartsWith(preamble3) ? Encoding.BigEndianUnicode.GetString(bytes, preamble3.Length, bytes.Length - preamble3.Length) : Encoding.UTF8.GetString(bytes);
        }
    }
}
