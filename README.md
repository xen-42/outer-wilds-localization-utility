# Outer Wilds Translation Mod Template

Use this project as a base for new Outer Wilds translation mods.

Based on [Outer Wilds Korean Translation](https://outerwildsmods.com/mods/outerwildskoreantranslation/) and [Outer Wilds Traditional Chinese Translation](https://outerwildsmods.com/mods/outerwildstraditionalchinesetranslation/).

## Getting started

- Create a base mod using the regular mod template.
- Add ILocalizationAPI as an interface in your mod with the following code:
```cs
public interface ILocalizationAPI
{
    void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath, string fontPath, Func<string, string> fixer);
    void RegisterLanguage(ModBehaviour mod, string name, string translationPath, string assetBundlePath, string fontPath);
    void RegisterLanguage(ModBehaviour mod, string name, string translationPath);
}
```

Access the utility mod in your mod code like so:
```cs
var api = ModHelper.Interaction.TryGetModApi<ILocalizationAPI>("xen.LocalizationUtility");
```

Call one of the RegisterLanguage functions to implement the mod. Adding a font is optional. A "fixer" function will take in a string and output a string where the characters have been correctly reformatted. This is necessary for certain languages, e.g. right-to-left languages like Arabic or Farsi.
