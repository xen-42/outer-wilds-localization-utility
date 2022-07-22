using OWML.ModHelper;
using System;

namespace LocalizationUtility
{
    public class LocalizationAPI : ILocalizationAPI
    {
        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath) 
            => LocalizationUtility.Instance.RegisterLanguage(mod, name, translationPath);
        public void AddLanguageFont(ModBehaviour mod, string name, string assetBundlePath, string fontPath)
            => LocalizationUtility.Instance.AddLanguageFont(mod, name, assetBundlePath, fontPath);
        public void AddLanguageFixer(string name, Func<string, string> fixer)
            => LocalizationUtility.Instance.AddLanguageFixer(name, fixer);
    }
}
