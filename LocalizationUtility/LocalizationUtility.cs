using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LocalizationUtility
{
    public class LocalizationUtility : ModBehaviour
    {
        public static LocalizationUtility Instance;

        private static readonly HashSet<TextTranslation.Language> vanillaLanguages = new(Enum.GetValues(typeof(TextTranslation.Language)).Cast<TextTranslation.Language>());
        public static TextTranslation.Language LanguageToReplace => Instance.GetLanguage().LanguageToReplace;

        internal Dictionary<string, CustomLanguage> _customLanguages = new();

        internal bool HasAnyCustomLanguages => _customLanguages.Count((pair) => pair.Value.IsCustom) > 0;

        public override object GetApi()
        {
            return new LocalizationAPI();
        }

        internal static bool IsVanillaLanguage() => IsVanillaLanguage(TextTranslation.Get().GetLanguage());

        internal static bool IsVanillaLanguage(TextTranslation.Language language) => TextTranslation.Language.UNKNOWN <= language && language <= TextTranslation.Language.TOTAL;

        public CustomLanguage GetLanguage() => GetLanguage(TextTranslation.Get().GetLanguage());

        public CustomLanguage GetLanguage(string languageName)
        {
            if (languageName != null && _customLanguages.TryGetValue(languageName, out var customLanguage)) {
                return customLanguage;
            }

            WriteError($"The language \"{languageName}\" is null");
            return null;
        }

        public bool TryGetLanguage(string languageName, out CustomLanguage customLanguage)
        {
            if (languageName != null && _customLanguages.TryGetValue(languageName, out customLanguage))
            {
                return true;
            }
            customLanguage = null;
            return false;
        }

        public CustomLanguage GetLanguage(TextTranslation.Language language)
        {
            if (name != null) {
                CustomLanguage customLanguage = _customLanguages.Values.FirstOrDefault(cl => cl.Language == language);
                if (customLanguage != null) {
                    return customLanguage;
                }
            }

            WriteError($"The language {language} is null");
            return null;
        }
        public bool TryGetLanguage(TextTranslation.Language language, out CustomLanguage customLanguage)
        {
            customLanguage = null;
            if (name != null)
            {
                customLanguage = _customLanguages.Values.FirstOrDefault(cl => cl.Language == language);
                if (customLanguage != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetLanguage(string languageName)
        {
            TextTranslation.s_theTable.SetLanguage(GetLanguage(languageName).Language);
        }

        public void RegisterLanguage(ModBehaviour mod, string languageName, string translationPath, TextTranslation.Language languageToReplace)
        {
            try
            {
                WriteLine($"Registering new language {languageName}");

                TextTranslation.Language newLanguage = EnumUtils.Create<TextTranslation.Language>(languageName);

                CustomLanguage customLanguage = new(languageName,true, newLanguage, languageToReplace);
                _customLanguages[languageName] = customLanguage;

                customLanguage.AddTranslation(mod.ModHelper.Manifest.ModFolderPath  + translationPath);
            }
            catch(Exception ex)
            {
                WriteError($"Failed to register language. {ex}");
            }
        }

        public void AddTranslation(ModBehaviour mod, string languageName, string translationPath)
        {
            try
            {
                WriteLine($"Adding translations to language {languageName}");

                if (TryGetLanguage(languageName, out var customLanguage))
                {
                    customLanguage.AddTranslation(mod.ModHelper.Manifest.ModFolderPath + translationPath);
                    return;
                }
                WriteError($"The custom language {languageName} isn't registered yet");

            }
            catch (Exception ex)
            {
                WriteError($"Failed to add translation to language {languageName}. {ex}");
            }
        }
        #region NonXMLTranslationAdders
        public void AddTranslation(string languageName, KeyValuePair<string, string>[] regularEntries, KeyValuePair<string, string>[] shipLogEntries, KeyValuePair<int, string>[] uiEntries)
        {
            try
            {
                WriteLine($"Adding translations to language {languageName}");

                if (TryGetLanguage(languageName, out var customLanguage))
                {
                    customLanguage.AddTranslation(regularEntries, shipLogEntries, uiEntries);
                    return;
                }
                WriteError($"The custom language {languageName} isn't registered yet");

            }
            catch (Exception ex)
            {
                WriteError($"Failed to add translation to language {languageName}. {ex}");
            }
        }
        public void AddRegularTranslation(string languageName, string commonKeyPrefix, params string[] entries)
        {
            AddRegularTranslation(languageName, CustomLanguage.GenerateKeyValuePairsForEntries(commonKeyPrefix, entries));
        }
        public void AddRegularTranslation(string languageName, params KeyValuePair<string, string>[] entries)
        {
            try
            {
                WriteLine($"Adding regular translations to language {languageName}");

                if (TryGetLanguage(languageName, out var customLanguage))
                {
                    customLanguage.AddEntriesToRegularTranslationTable(entries);
                    return;
                }
                WriteError($"The custom language {languageName} isn't registered yet");

            }
            catch (Exception ex)
            {
                WriteError($"Failed to add regular translations to language {languageName}. {ex}");
            }
        }
        public void AddShiplogTranslation(string languageName, string commonKeyPrefix, params string[] entries)
        {
            AddShiplogTranslation(languageName, CustomLanguage.GenerateKeyValuePairsForEntries(commonKeyPrefix, entries));
        }
        public void AddShiplogTranslation(string languageName, params KeyValuePair<string, string>[] entries)
        {
            try
            {
                WriteLine($"Adding shiplog translations to language {languageName}");

                if (TryGetLanguage(languageName, out var customLanguage))
                {
                    customLanguage.AddEntriesToShiplogTranslationTable(entries);
                    return;
                }
                WriteError($"The custom language {languageName} isn't registered yet");

            }
            catch (Exception ex)
            {
                WriteError($"Failed to add shiplog translations to language {languageName}. {ex}");
            }
        }
        public void AddUITranslation(string languageName, params KeyValuePair<int, string>[] entries)
        {
            try
            {
                WriteLine($"Adding UI translations to language {languageName}");

                if (TryGetLanguage(languageName, out var customLanguage))
                {
                    customLanguage.AddEntriesToUITranslationTable(entries);
                    return;
                }
                WriteError($"The custom language {languageName} isn't registered yet");

            }
            catch (Exception ex)
            {
                WriteError($"Failed to add UI translations to language {languageName}. {ex}");
            }
        }
        #endregion
        public void AddLanguageFont(ModBehaviour mod, string languageName, string assetBundlePath, string fontPath)
        {
            try
            {
                _customLanguages[languageName].AddFont(assetBundlePath, fontPath, mod);
            }
            catch (Exception ex)
            {
                WriteError($"Failed to add font to language. {ex}");
            }
        }

        public void AddLanguageFixer(string languageName, Func<string, string> fixer)
        {
            try
            {
                _customLanguages[languageName].AddFixer(fixer);
            }
            catch (Exception ex)
            {
                WriteError($"Failed to add fixer to language. {ex}");
            }
        }

        public void SetLanguageDefaultFontSpacing(string languageName, float defaultFontSpacing)
        {
            try
            {
                _customLanguages[languageName].SetDefaultFontSpacing(defaultFontSpacing);
            }
            catch (Exception ex)
            {
                WriteError($"Failed to set default font spacing for language. {ex}");
            }
        }

        public void SetLanguageFontSizeModifier(string languageName, float fontSizeModifier)
        {
            try
            {
                _customLanguages[languageName].SetFontSizeModifier(fontSizeModifier);
            }
            catch (Exception ex)
            {
                WriteError($"Failed to set font size modifier for language. {ex}");
            }
        }

        private void Awake()
        {
            Instance = this;

            foreach(var lang in vanillaLanguages) //Adds the vanilla languages so we can add translations to it with AddTranslation
            {
                string languageName = lang.ToString();
                _customLanguages[languageName] = new CustomLanguage(languageName,false, lang, lang);
            }
        }

        private void Start()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            WriteLine($"Translation mod {nameof(LocalizationUtility)} is loaded!");
        }

        public static void WriteLine(string msg, MessageType type = MessageType.Info)
        {
            Instance.ModHelper.Console.WriteLine($"{type}: {msg}", type);
        }

        public static void WriteError(string msg) => WriteLine(msg, MessageType.Error);

        public static LanguageSaveFile Load() => Instance.ModHelper.Storage.Load<LanguageSaveFile>("languageSave.json") ?? LanguageSaveFile.DEFAULT;

        public static void Save(LanguageSaveFile save) => Instance.ModHelper.Storage.Save(save ?? LanguageSaveFile.DEFAULT, "languageSave.json");
    }
}
