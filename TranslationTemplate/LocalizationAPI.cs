using OWML.ModHelper;
using System;

namespace TranslationTemplate
{
    public class LocalizationAPI : ILocalizationAPI
    {
        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath, string fontPath, Func<string, string> fixer)
            => TranslationTemplate.Instance.RegisterLanguage(mod, name, translationPath, assetBundlePath, fontPath, fixer);

        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath, string fontPath) 
            => TranslationTemplate.Instance.RegisterLanguage(mod, name, translationPath, assetBundlePath, fontPath);

        public void RegisterLanguage(ModBehaviour mod, string name, string translationPath) 
            => TranslationTemplate.Instance.RegisterLanguage(mod, name, translationPath);
    }
}
