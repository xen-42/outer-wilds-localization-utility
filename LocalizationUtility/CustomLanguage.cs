using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace LocalizationUtility
{
    public class CustomLanguage
    {
        public string Name { get; private set; }
        public TextTranslation.Language Language { get; private set; }
        //public string TranslationPath { get; private set; }
        public Font Font { get; private set; }
        public Func<string, string> Fixer { get; private set; }
        public float DefaultFontSpacing { get; private set; } = 1;
        public float FontSizeModifier { get; private set; } = 1;
        public TextTranslation.Language LanguageToReplace { get; private set; }
        public bool IsCustom { get; private set; }

        public TextTranslation.TranslationTable_XML TranslationTable { get; private set; }

        public CustomLanguage(string name, bool isCustom, TextTranslation.Language language, TextTranslation.Language languageToReplace)
        {
            Name = name;
            IsCustom = isCustom;

            Language = language;
            LanguageToReplace = languageToReplace;

            //TranslationPath = ((mod != null)? mod.ModHelper.Manifest.ModFolderPath : "" )+ translationPath;
            TranslationTable = new TextTranslation.TranslationTable_XML();

            //AddTranslation(TranslationPath);
        }

        public void AddFont(string assetBundlePath, string fontPath, ModBehaviour mod)
        {
            try
            {
                Font = AssetBundle.LoadFromFile(mod.ModHelper.Manifest.ModFolderPath + assetBundlePath).LoadAsset<Font>(fontPath);
            }
            catch (Exception) { };

            if (Font == null) LocalizationUtility.WriteError($"Couldn't load font at {assetBundlePath} in bundle {fontPath}");
        }

        public void AddFixer(Func<string, string> fixer)
        {
            Fixer = fixer;
        }

        public void SetDefaultFontSpacing(float defaultFontSpacing)
        {
            DefaultFontSpacing = defaultFontSpacing;
        }

        public void SetFontSizeModifier(float fontSizeModifier)
        {
            FontSizeModifier = fontSizeModifier;
        }


        public void AddTranslation(string translationPath)
        {
            LocalizationUtility.WriteLine($"Storing translation for {Language} from {translationPath}");
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(ReadAndRemoveByteOrderMarkFromPath(translationPath));

                var translationTableNode = xmlDoc.SelectSingleNode("TranslationTable_XML");

                if (translationTableNode == null)
                {
                    LocalizationUtility.WriteError($"TranslationTable_XML could not be found in translation file in {translationPath} for language {Name}");
                    return;
                }

                // Add regular text to the table
                foreach (XmlNode node in translationTableNode.SelectNodes("entry"))
                {
                    var key = node.SelectSingleNode("key").InnerText;
                    var value = node.SelectSingleNode("value").InnerText;

                    if (Fixer != null) value = Fixer(value);

                    TranslationTable.table.Add(new TextTranslation.TranslationTableEntry(key, value));
                }

                // Add ship log entries
                foreach (XmlNode node in translationTableNode.SelectSingleNode("table_shipLog").SelectNodes("TranslationTableEntry"))
                {
                    var key = node.SelectSingleNode("key").InnerText;
                    var value = node.SelectSingleNode("value").InnerText;

                    if (Fixer != null) value = Fixer(value);

                    TranslationTable.table_shipLog.Add(new TextTranslation.TranslationTableEntry(key, value));
                }

                // Add UI
                foreach (XmlNode node in translationTableNode.SelectSingleNode("table_ui").SelectNodes("TranslationTableEntryUI"))
                {
                    // Keys for UI are all ints
                    var key = int.Parse(node.SelectSingleNode("key").InnerText);
                    var value = node.SelectSingleNode("value").InnerText;

                    if (Fixer != null) value = Fixer(value);

                    TranslationTable.table_ui.Add(new TextTranslation.TranslationTableEntryUI(key, value));
                }
            }
            catch (Exception e)
            {
                LocalizationUtility.WriteError($"Couldn't load translation for language {Name} from file in {translationPath}: {e.Message}{e.StackTrace}");
            }
        }

        #region NonXMLTranslationAdders
        public static KeyValuePair<string, string>[] GenerateKeyValuePairsForEntries(string commonKeyPrefix, params string[] entries)
        {
            KeyValuePair<string, string>[] pairs = new KeyValuePair<string, string>[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                pairs[i] = new KeyValuePair<string, string>(commonKeyPrefix + entries[i], entries[i]);
            }
            return pairs;
        }
        public void AddTranslation(KeyValuePair<string, string>[] regularEntries, KeyValuePair<string, string>[] shipLogEntries, KeyValuePair<int, string>[] uiEntries)
        {
            LocalizationUtility.WriteLine($"Storing translations for {Language}");
            try
            {
                // Add regular text to the table
                foreach (var pair in regularEntries)
                {
                    var key = pair.Key;
                    var value = pair.Value;

                    if (Fixer != null) value = Fixer(value);

                    TranslationTable.table.Add(new TextTranslation.TranslationTableEntry(key, value));
                }

                // Add ship log entries
                foreach (var pair in shipLogEntries)
                {
                    var key = pair.Key;
                    var value = pair.Value;

                    if (Fixer != null) value = Fixer(value);

                    TranslationTable.table_shipLog.Add(new TextTranslation.TranslationTableEntry(key, value));
                }

                // Add UI
                foreach (var pair in uiEntries)
                {
                    var key = pair.Key;
                    var value = pair.Value;

                    if (Fixer != null) value = Fixer(value);

                    TranslationTable.table_ui.Add(new TextTranslation.TranslationTableEntryUI(key, value));
                }
            }
            catch (Exception e)
            {
                LocalizationUtility.WriteError($"Couldn't store translation for language {Name}: {e.Message}{e.StackTrace}");
            }
        }
        public void AddEntriesToRegularTranslationTable(params KeyValuePair<string, string>[] entries)
        {
            LocalizationUtility.WriteLine($"Storing translation for {Language} for regular translations");
            try
            {
                // Add regular text to the table
                foreach (var entry in entries)
                {
                    var key = entry.Key;
                    var value = entry.Value;

                    if (Fixer != null) value = Fixer(value);

                    TranslationTable.table.Add(new TextTranslation.TranslationTableEntry(key, value));
                }
            }
            catch (Exception e)
            {
                LocalizationUtility.WriteError($"Couldn't store translations for language {Name}: {e.Message}{e.StackTrace}");
            }
        }
        public void AddEntriesToShiplogTranslationTable(params KeyValuePair<string, string>[] entries)
        {
            LocalizationUtility.WriteLine($"Storing translation for {Language} for shiplog translations");
            try
            {
                // Add ship log entries
                foreach (var entry in entries)
                {
                    var key = entry.Key;
                    var value = entry.Value;

                    if (Fixer != null) value = Fixer(value);

                    TranslationTable.table_shipLog.Add(new TextTranslation.TranslationTableEntry(key, value));
                }
            }
            catch (Exception e)
            {
                LocalizationUtility.WriteError($"Couldn't add translation for language {Name}: {e.Message}{e.StackTrace}");
            }
        }

        public void AddEntriesToUITranslationTable(params KeyValuePair<int, string>[] entries)
        {
            LocalizationUtility.WriteLine($"Storing translation for {Language} for UI translations");
            try
            {
                // Add UI
                foreach (var entry in entries)
                {
                    var key = entry.Key;
                    var value = entry.Value;

                    if (Fixer != null) value = Fixer(value);

                    TranslationTable.table_ui.Add(new TextTranslation.TranslationTableEntryUI(key, value));
                }
            }
            catch (Exception e)
            {
                LocalizationUtility.WriteError($"Couldn't add translation for language {Name}: {e.Message}{e.StackTrace}");
            }
        }
        #endregion

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
