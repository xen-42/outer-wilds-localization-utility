using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LocalizationUtility
{
    public class LocalizationUtility : ModBehaviour
    {
        public static LocalizationUtility Instance;

        public static TextTranslation.Language languageToReplace = TextTranslation.Language.ENGLISH;

        private Dictionary<string, CustomLanguage> _customLanguages = new();
        private string currentLanguage;

        public override object GetApi()
        {
            return new LocalizationAPI();
        }

        public CustomLanguage GetLanguage()
        {
            if (currentLanguage != null && _customLanguages.TryGetValue(currentLanguage, out var customLanguage)) {
                return customLanguage;
            }

            WriteError("The selected language is null");
            return null;
        }

        public void SetLanguage(string name)
        {
            currentLanguage = name;
            TextTranslation.s_theTable.SetLanguage(TextTranslation.Language.ENGLISH);
        }

        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath)
        {
            try
            {
                WriteLine($"Registering new language {name}");

                _customLanguages[name] = new CustomLanguage(name, translationPath, mod);

                // For now it only supports one language mod at a time
                currentLanguage = name;
            }
            catch(Exception ex)
            {
                WriteError($"Failed to register language. {ex}");
            }
        }

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

        private void Awake()
        {
            Instance = this;
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
    }
}
