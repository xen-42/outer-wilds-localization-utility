using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationTemplate
{
    public interface ILocalizationAPI
    {
        void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath, string fontPath);
        void RegisterLanguage(ModBehaviour mod, string name, string translationPath);
    }
}
