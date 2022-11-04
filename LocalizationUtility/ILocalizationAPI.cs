using OWML.ModHelper;
using System;
using System.Collections.Generic;

namespace LocalizationUtility
{
    public interface ILocalizationAPI
    {
        void RegisterLanguage(ModBehaviour mod, string name, string translationPath);
        void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string languageToReplace);
        void AddTranslation(ModBehaviour mod, string name, string translationPath);
        public void AddRegularTranslation(string name, string key, string value);
        public void AddRegularTranslation(string name, string commonKeyPrefix, params string[] entries);
        public void AddRegularTranslation(string name, params KeyValuePair<string, string>[] entries);
        public void AddShiplogTranslation(string name, string key, string value);
        public void AddShiplogTranslation(string name, string commonKeyPrefix, params string[] entries);
        public void AddShiplogTranslation(string name, params KeyValuePair<string, string>[] entries);
        public void AddUITranslation(string name, int key, string value);
        public void AddUITranslation(string name, params KeyValuePair<int, string>[] entries);
        void AddLanguageFont(ModBehaviour mod, string name, string assetBundlePath, string fontPath);
        void AddLanguageFixer(string name, Func<string, string> fixer);
        void SetLanguageDefaultFontSpacing(string name, float defaultFontSpacing);
        void SetLanguageFontSizeModifier(string name, float fontSizeModifier);
    }
}
