using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TranslationTemplate
{
    public class CustomLanguage
    {
        public string Name { get; private set; }
        public string TranslationPath { get; private set; }
        public Font Font { get; private set; }

        private string _fontAssetBundlePath;
        private string _fontAssetPath;

        public CustomLanguage(string name, string translationPath, string fontAssetBundlePath, string fontAssetPath, ModBehaviour mod)
        {
            Name = name;
            TranslationPath = mod.ModHelper.Manifest.ModFolderPath + translationPath;
            _fontAssetBundlePath = mod.ModHelper.Manifest.ModFolderPath + fontAssetBundlePath;
            _fontAssetPath = fontAssetPath;

            Font = AssetBundle.LoadFromFile(_fontAssetBundlePath).LoadAsset<Font>(_fontAssetPath);
        }

        public CustomLanguage(string name, string translationPath, ModBehaviour mod)
        {
            Name = name;
            TranslationPath = mod.ModHelper.Manifest.ModFolderPath + translationPath;
        }
    }
}
