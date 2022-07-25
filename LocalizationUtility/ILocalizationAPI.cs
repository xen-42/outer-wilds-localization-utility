using OWML.ModHelper;
using System;

namespace LocalizationUtility
{
    public interface ILocalizationAPI
    {
        void RegisterLanguage(ModBehaviour mod, string name, string translationPath);
        void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string languageToReplace);
        void AddLanguageFont(ModBehaviour mod, string name, string assetBundlePath, string fontPath);
        void AddLanguageFixer(string name, Func<string, string> fixer);
        void SetLanguageDefaultFontSpacing(string name, float defaultFontSpacing);
        void SetLanguageFontSizeModifier(string name, float fontSizeModifier);
    }
}
