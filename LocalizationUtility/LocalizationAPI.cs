using OWML.ModHelper;
using System;

namespace LocalizationUtility
{
    public class LocalizationAPI : ILocalizationAPI
    {
        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath) 
            => LocalizationUtility.Instance.RegisterLanguage(mod, name, translationPath, TextTranslation.Language.ENGLISH);
        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string languageToReplace)
            => LocalizationUtility.Instance.RegisterLanguage(mod, name, translationPath, (TextTranslation.Language) Enum.Parse(typeof(TextTranslation.Language), languageToReplace, true));
        public void AddTranslation(ModBehaviour mod, string name, string translationPath)
            => LocalizationUtility.Instance.AddTranslation(mod, name, translationPath);
        public void AddLanguageFont(ModBehaviour mod, string name, string assetBundlePath, string fontPath)
            => LocalizationUtility.Instance.AddLanguageFont(mod, name, assetBundlePath, fontPath);
        public void AddLanguageFixer(string name, Func<string, string> fixer)
            => LocalizationUtility.Instance.AddLanguageFixer(name, fixer);
        public void SetLanguageDefaultFontSpacing(string name, float defaultFontSpacing)
            => LocalizationUtility.Instance.SetLanguageDefaultFontSpacing(name, defaultFontSpacing);
        public void SetLanguageFontSizeModifier(string name, float fontSizeModifier)
            => LocalizationUtility.Instance.SetLanguageFontSizeModifier(name, fontSizeModifier);
    }
}
