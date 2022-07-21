using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TranslationTemplate
{
    public class TranslationTemplate : ModBehaviour
    {
        public static TranslationTemplate Instance;

        public static TextTranslation.Language languageToReplace = TextTranslation.Language.ENGLISH;

        private Dictionary<string, CustomLanguage> _customLanguages = new();
        private string currentLanguage = "عربى";

        public CustomLanguage GetLanguage()
        {
            return _customLanguages[currentLanguage];
        }

        public void SetLanguage(string name)
        {
            currentLanguage = name;
            TextTranslation.s_theTable.SetLanguage(TextTranslation.Language.ENGLISH);
        }

        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath = "", string fontPath = "", Func<string, string> fixer = null)
        {
            if (string.IsNullOrEmpty(assetBundlePath) || string.IsNullOrEmpty(fontPath))
            {
                _customLanguages[name] = new CustomLanguage(name, translationPath, mod);
            }
            else
            {
                _customLanguages[name] = new CustomLanguage(name, translationPath, assetBundlePath, fontPath, fixer, mod);
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            WriteLine($"Translation mod {nameof(TranslationTemplate)} is loaded!");

            RegisterLanguage(this, "Czech", "assets/czech.xml", "assets/aileron-black", "Assets/aileron-black.oft");
            RegisterLanguage(this, "عربى", "assets/Translations.xml", "assets/b_mitra_0", "Assets/B_MITRA_0.TTF", ArabicSupport.ArabicFixer.Fix);
        }

        public static void WriteLine(string msg, MessageType type = MessageType.Info)
        {
            Instance.ModHelper.Console.WriteLine($"{type}: {msg}", type);
        }

        public static void WriteError(string msg) => WriteLine(msg, MessageType.Error);
    }
}
