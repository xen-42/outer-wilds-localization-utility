using OWML.ModHelper;
using System;
using System.Collections.Generic;

namespace LocalizationUtility
{
    public class LocalizationAPI : ILocalizationAPI
    {
        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath) 
            => LocalizationUtility.Instance.RegisterLanguage(mod, name, translationPath, TextTranslation.Language.ENGLISH);
        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string languageToReplace)
            => LocalizationUtility.Instance.RegisterLanguage(mod, name, translationPath, (TextTranslation.Language) Enum.Parse(typeof(TextTranslation.Language), languageToReplace, true));
        public void AddTranslation(string name, KeyValuePair<string, string>[] regularEntries, KeyValuePair<string, string>[] shipLogEntries, KeyValuePair<int, string>[] uiEntries)
            => LocalizationUtility.Instance.AddTranslation(name, regularEntries, shipLogEntries, uiEntries);
        public void AddTranslation(ModBehaviour mod, string name, string translationPath)
            => LocalizationUtility.Instance.AddTranslation(mod, name, translationPath);
        #region AddRegularTranslation
        public void AddRegularTranslation(string name, string key, string value)
            => LocalizationUtility.Instance.AddRegularTranslation(name, new KeyValuePair<string, string>(key, value));
        public void AddRegularTranslation(string name, string commonKeyPrefix, params string[] entries)
            => LocalizationUtility.Instance.AddRegularTranslation(name, commonKeyPrefix, entries);
        public void AddRegularTranslation(string name, params KeyValuePair<string, string>[] entries)
            => LocalizationUtility.Instance.AddRegularTranslation(name,entries);
        #endregion
        #region AddShiplogTranslation
        public void AddShiplogTranslation(string name, string key, string value)
            => LocalizationUtility.Instance.AddShiplogTranslation(name, new KeyValuePair<string, string>(key, value));
        public void AddShiplogTranslation(string name, string commonKeyPrefix, params string[] entries)
            => LocalizationUtility.Instance.AddShiplogTranslation(name, commonKeyPrefix, entries);
        public void AddShiplogTranslation(string name, params KeyValuePair<string, string>[] entries)
            => LocalizationUtility.Instance.AddShiplogTranslation(name, entries);
        #endregion
        #region AddUITranslation
        public void AddUITranslation(string name, int key, string value)
            => LocalizationUtility.Instance.AddUITranslation(name, new KeyValuePair<int, string>(key, value));
        public void AddUITranslation(string name, params KeyValuePair<int, string>[] entries)
            => LocalizationUtility.Instance.AddUITranslation(name, entries);
        #endregion
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
