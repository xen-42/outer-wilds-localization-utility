using OWML.ModHelper;
using System;
using System.Collections.Generic;

namespace LocalizationUtility
{
    public class LocalizationAPI : ILocalizationAPI
    {
        public void RegisterLanguage(ModBehaviour mod, string languageName, string translationPath) 
            => LocalizationUtility.Instance.RegisterLanguage(mod, languageName, translationPath, TextTranslation.Language.ENGLISH);
        public void RegisterLanguage(ModBehaviour mod, string languageName, string translationPath, string languageToReplace)
            => LocalizationUtility.Instance.RegisterLanguage(mod, languageName, translationPath, (TextTranslation.Language) Enum.Parse(typeof(TextTranslation.Language), languageToReplace, true));
        public void AddTranslation(string languageName, KeyValuePair<string, string>[] regularEntries, KeyValuePair<string, string>[] shipLogEntries, KeyValuePair<int, string>[] uiEntries)
            => LocalizationUtility.Instance.AddTranslation(languageName, regularEntries, shipLogEntries, uiEntries);
        public void AddTranslation(ModBehaviour mod, string languageName, string translationPath)
            => LocalizationUtility.Instance.AddTranslation(mod, languageName, translationPath);
        #region AddRegularTranslation
        public void AddRegularTranslation(string languageName, string key, string value)
            => LocalizationUtility.Instance.AddRegularTranslation(languageName, new KeyValuePair<string, string>(key, value));
        public void AddRegularTranslation(string languageName, string commonKeyPrefix, params string[] entries)
            => LocalizationUtility.Instance.AddRegularTranslation(languageName, commonKeyPrefix, entries);
        public void AddRegularTranslation(string languageName, params KeyValuePair<string, string>[] entries)
            => LocalizationUtility.Instance.AddRegularTranslation(languageName,entries);
        #endregion
        #region AddShiplogTranslation
        public void AddShiplogTranslation(string languageName, string key, string value)
            => LocalizationUtility.Instance.AddShiplogTranslation(languageName, new KeyValuePair<string, string>(key, value));
        public void AddShiplogTranslation(string languageName, string commonKeyPrefix, params string[] entries)
            => LocalizationUtility.Instance.AddShiplogTranslation(languageName, commonKeyPrefix, entries);
        public void AddShiplogTranslation(string languageName, params KeyValuePair<string, string>[] entries)
            => LocalizationUtility.Instance.AddShiplogTranslation(languageName, entries);
        #endregion
        #region AddUITranslation
        public void AddUITranslation(string languageName, int key, string value)
            => LocalizationUtility.Instance.AddUITranslation(languageName, new KeyValuePair<int, string>(key, value));
        public void AddUITranslation(string languageName, params KeyValuePair<int, string>[] entries)
            => LocalizationUtility.Instance.AddUITranslation(languageName, entries);
        #endregion
        public void AddLanguageFont(ModBehaviour mod, string languageName, string assetBundlePath, string fontPath)
            => LocalizationUtility.Instance.AddLanguageFont(mod, languageName, assetBundlePath, fontPath);
        public void AddLanguageFixer(string languageName, Func<string, string> fixer)
            => LocalizationUtility.Instance.AddLanguageFixer(languageName, fixer);
        public void SetLanguageDefaultFontSpacing(string languageName, float defaultFontSpacing)
            => LocalizationUtility.Instance.SetLanguageDefaultFontSpacing(languageName, defaultFontSpacing);
        public void SetLanguageFontSizeModifier(string languageName, float fontSizeModifier)
            => LocalizationUtility.Instance.SetLanguageFontSizeModifier(languageName, fontSizeModifier);
    }
}
