# Outer Wilds Translation Mod Template

This project is meant to have common code required for translation mods.

Based on [Outer Wilds Korean Translation](https://outerwildsmods.com/mods/outerwildskoreantranslation/) and [Outer Wilds Traditional Chinese Translation](https://outerwildsmods.com/mods/outerwildstraditionalchinesetranslation/).

#### This repo contains the base game English translation file (Translation.xml) which can be used as a template for any translation mod. Just translate the <value> entries of the XML and leave the original English <key> entries alone.

## Getting started

1. Create a base mod using [the regular mod template.](https://github.com/Raicuparta/ow-mod-template). 
2. Rename all files to match the mod name (for example let's say we are making the DothrakiTranslation mod):
   1. Instead of `ModTemplate/ModTemplate.cs`, rename files to `DothrakiTranslation/DothrakiTranslation.cs`. (This also applies to `ModTemplate.csproj.user`.)
   2. Replace every instance of `ModTemplate` with `DothrakiTranslation` in the script texts (should be just the `ModTemplate.cs`).
   3. Fix up `filename` in `manifest.json` accordingly.
3. Add a dictionary entry to the end of the `manifest.json` dict that specifies the dependency for Outer Wilds Mod Manager:
    ```json
    "dependencies": [
        "xen.LocalizationUtility"
    ]
    ```

    Note that this will not automatically install the dependency, however the Outer Wilds Mod Manager will prompt users to install and enable the dependency when they enable your translation mod.
4. Add a file called `ILocalizationAPI.cs` (a C# interface) into your mod directory, with the following content - replace `DothrakiTranslation` with the name of your translation, of course:
    ```cs
    using OWML.ModHelper;
    using System;

    namespace DothrakiTranslation
    {
        public interface ILocalizationAPI
        {
            void RegisterLanguage(ModBehaviour mod, string name, string translationPath);
            void AddLanguageFont(ModBehaviour mod, string name, string assetBundlePath, string fontPath);
            void AddLanguageFixer(string name, Func<string, string> fixer);
        }
    }
    ```
5. In your mod code (`DothrakiTranslation.cs`), access the utility mod like so:
    ```cs
    namespace DothrakiTranslation
    {
        public class DothrakiTranslation : ModBehaviour
        {
            public static DothrakiTranslation Instance;

            private void Start()
            {
                var api = ModHelper.Interaction.TryGetModApi<ILocalizationAPI>("xen.LocalizationUtility");
                api.RegisterLanguage(this, "Dothraki", "assets/Translation.xml");
            }
        }
    }
    ```
    This assumes that the XML file with original text and your translations is in the `assets/Translation.xml` file.
6. Optionally, add a font or a fixer function with `api.AddLanguageFont` or `api.AddLanguageFixer` underneath the `api.RegisterLanguage` line. Adding a font is optional. A "fixer" function will take in a string and output a string where the characters have been correctly reformatted. This is necessary for certain languages, e.g. right-to-left languages like Arabic or Farsi.
