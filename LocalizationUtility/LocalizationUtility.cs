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

        internal Dictionary<string, CustomLanguage> _customLanguages = new();

        internal bool hasAnyCustomLanguages => _customLanguages.Count > 0;

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

        public void SetLanguage(string name)
        {
            TextTranslation.s_theTable.SetLanguage(GetLanguage(name).Language);
        }

        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath)
        {
            try
            {
                WriteLine($"Registering new language {name}");

                TextTranslation.Language newLanguage = (hasAnyCustomLanguages ? _customLanguages.Values.Max(cl => cl.Language) : vanillaLanguages.Max()) + 1;

                _customLanguages[name] = new CustomLanguage(name, newLanguage, translationPath, mod);
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

        public static LanguageSaveFile Load() => Instance.ModHelper.Storage.Load<LanguageSaveFile>("langaugeSave.json") ?? LanguageSaveFile.DEFAULT;

        public static void Save(LanguageSaveFile save) => Instance.ModHelper.Storage.Save(save ?? LanguageSaveFile.DEFAULT, "langaugeSave.json");
    }
}
