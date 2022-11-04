using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LocalizationUtility
{
    public class LocalizationUtility : ModBehaviour
    {
        public static LocalizationUtility Instance;

        private static readonly HashSet<TextTranslation.Language> vanillaLanguages = new HashSet<TextTranslation.Language>(Enum.GetValues(typeof(TextTranslation.Language)).Cast<TextTranslation.Language>());
        
        public static TextTranslation.Language languageToReplace => Instance.GetLanguage().LanguageToReplace;

        internal Dictionary<string, CustomLanguage> _customLanguages = new();

        internal bool hasAnyCustomLanguages => _customLanguages.Count((pair) => pair.Value.IsCustom) > 0;

        public override object GetApi()
        {
            return new LocalizationAPI();
        }

        internal static bool IsVanillaLanguage() => IsVanillaLanguage(TextTranslation.Get().GetLanguage());

        internal static bool IsVanillaLanguage(TextTranslation.Language language) => vanillaLanguages.Contains(language);

        public CustomLanguage GetLanguage() => GetLanguage(TextTranslation.Get().GetLanguage());

        public CustomLanguage GetLanguage(string name)
        {
            if (name != null && _customLanguages.TryGetValue(name, out var customLanguage)) {
                return customLanguage;
            }

            WriteError($"The language \"{name}\" is null");
            return null;
        }

        public bool TryGetLanguage(string name, out CustomLanguage customLanguage)
        {
            if (name != null && _customLanguages.TryGetValue(name, out customLanguage))
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

        public void SetLanguage(string name)
        {
            TextTranslation.s_theTable.SetLanguage(GetLanguage(name).Language);
        }

        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath, TextTranslation.Language languageToReplace)
        {
            try
            {
                WriteLine($"Registering new language {name}");

                TextTranslation.Language newLanguage = (hasAnyCustomLanguages ? _customLanguages.Values.Max(cl => cl.Language) : vanillaLanguages.Max()) + 1;

                CustomLanguage customLanguage = new CustomLanguage(name,true, newLanguage, translationPath, mod, languageToReplace);
                _customLanguages[name] = customLanguage;

                TextTranslationPatches.AddNewTranslation(customLanguage, customLanguage.TranslationPath);
            }
            catch(Exception ex)
            {
                WriteError($"Failed to register language. {ex}");
            }
        }

        public void AddTranslation(ModBehaviour mod, string name, string translationPath)
        {
            try
            {
                WriteLine($"Adding translations to language {name}");

                if (TryGetLanguage(name, out var customLanguage))
                {
                    TextTranslationPatches.AddNewTranslation(customLanguage, mod.ModHelper.Manifest.ModFolderPath + translationPath);
                    return;
                }
                WriteError($"The custom language {name} isn't registered yet");

            }
            catch (Exception ex)
            {
                WriteError($"Failed to add translation to language {name}. {ex}");
            }
        }
        #region NonXMLTranslationAdders
        public void AddTranslation(string name, KeyValuePair<string, string>[] regularEntries, KeyValuePair<string, string>[] shipLogEntries, KeyValuePair<int, string>[] uiEntries)
        {
            try
            {
                WriteLine($"Adding translations to language {name}");

                if (TryGetLanguage(name, out var customLanguage))
                {
                    TextTranslationPatches.AddNewTranslation(customLanguage, regularEntries, shipLogEntries, uiEntries);
                    return;
                }
                WriteError($"The custom language {name} isn't registered yet");

            }
            catch (Exception ex)
            {
                WriteError($"Failed to add translation to language {name}. {ex}");
            }
        }
        public void AddRegularTranslation(string name, string commonKeyPrefix, params string[] entries)
        {
            AddRegularTranslation(name, TextTranslationPatches.GenerateKeyValuePairsForEntries(commonKeyPrefix, entries));
        }
        public void AddRegularTranslation(string name, params KeyValuePair<string, string>[] entries)
        {
            try
            {
                WriteLine($"Adding regular translations to language {name}");

                if (TryGetLanguage(name, out var customLanguage))
                {
                    TextTranslationPatches.AddEntriesToRegularTranslationTable(customLanguage, entries);
                    return;
                }
                WriteError($"The custom language {name} isn't registered yet");

            }
            catch (Exception ex)
            {
                WriteError($"Failed to add regular translations to language {name}. {ex}");
            }
        }
        public void AddShiplogTranslation(string name, string commonKeyPrefix, params string[] entries)
        {
            AddShiplogTranslation(name, TextTranslationPatches.GenerateKeyValuePairsForEntries(commonKeyPrefix, entries));
        }
        public void AddShiplogTranslation(string name, params KeyValuePair<string, string>[] entries)
        {
            try
            {
                WriteLine($"Adding shiplog translations to language {name}");

                if (TryGetLanguage(name, out var customLanguage))
                {
                    TextTranslationPatches.AddEntriesToShiplogTranslationTable(customLanguage, entries);
                    return;
                }
                WriteError($"The custom language {name} isn't registered yet");

            }
            catch (Exception ex)
            {
                WriteError($"Failed to add shiplog translations to language {name}. {ex}");
            }
        }
        public void AddUITranslation(string name, params KeyValuePair<int, string>[] entries)
        {
            try
            {
                WriteLine($"Adding UI translations to language {name}");

                if (TryGetLanguage(name, out var customLanguage))
                {
                    TextTranslationPatches.AddEntriesToUITranslationTable(customLanguage, entries);
                    return;
                }
                WriteError($"The custom language {name} isn't registered yet");

            }
            catch (Exception ex)
            {
                WriteError($"Failed to add UI translations to language {name}. {ex}");
            }
        }
        #endregion
        public void AddLanguageFont(ModBehaviour mod, string name, string assetBundlePath, string fontPath)
        {
            try
            {
                _customLanguages[name].AddFont(assetBundlePath, fontPath, mod);
            }
            catch (Exception ex)
            {
                WriteError($"Failed to add font to language. {ex}");
            }
        }

        public void AddLanguageFixer(string name, Func<string, string> fixer)
        {
            try
            {
                _customLanguages[name].AddFixer(fixer);
            }
            catch (Exception ex)
            {
                WriteError($"Failed to add fixer to language. {ex}");
            }
        }

        public void SetLanguageDefaultFontSpacing(string name, float defaultFontSpacing)
        {
            try
            {
                _customLanguages[name].SetDefaultFontSpacing(defaultFontSpacing);
            }
            catch (Exception ex)
            {
                WriteError($"Failed to set default font spacing for language. {ex}");
            }
        }

        public void SetLanguageFontSizeModifier(string name, float fontSizeModifier)
        {
            try
            {
                _customLanguages[name].SetFontSizeModifier(fontSizeModifier);
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
                _customLanguages[languageName] = new CustomLanguage(languageName,false, lang, "", null, lang);
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
