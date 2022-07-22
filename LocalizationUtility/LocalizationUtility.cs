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

        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath = "", string fontPath = "", Func<string, string> fixer = null)
        {
            try
            {
                WriteLine($"Registering new language {name}");

                if (string.IsNullOrEmpty(assetBundlePath) || string.IsNullOrEmpty(fontPath))
                {
                    _customLanguages[name] = new CustomLanguage(name, translationPath, mod);
                }
                else
                {
                    _customLanguages[name] = new CustomLanguage(name, translationPath, assetBundlePath, fontPath, fixer, mod);
                }

                // For now it only supports one language mod at a time
                currentLanguage = name;
            }
            catch(Exception ex)
            {
                WriteError($"Failed to register language. {ex.Message} {ex.StackTrace}");
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
