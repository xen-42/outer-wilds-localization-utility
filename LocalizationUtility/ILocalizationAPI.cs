using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationUtility
{
    public interface ILocalizationAPI
    {
        void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath, string fontPath, Func<string, string> fixer);
        void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath, string fontPath);
        void RegisterLanguage(ModBehaviour mod, string name, string translationPath);
    }
}
