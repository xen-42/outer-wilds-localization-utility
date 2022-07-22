using OWML.ModHelper;
using System;

namespace LocalizationUtility
{
    public class LocalizationAPI : ILocalizationAPI
    {
        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath, string fontPath, Func<string, string> fixer)
            => LocalizationUtility.Instance.RegisterLanguage(mod, name, translationPath, assetBundlePath, fontPath, fixer);

        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath, string fontPath) 
            => LocalizationUtility.Instance.RegisterLanguage(mod, name, translationPath, assetBundlePath, fontPath);

        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath) 
            => LocalizationUtility.Instance.RegisterLanguage(mod, name, translationPath);
    }
}
