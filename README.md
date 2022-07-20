# Outer Wilds Translation Mod Template

Use this project as a base for new Outer Wilds translation mods.

Based on [Outer Wilds Korean Translation](https://outerwildsmods.com/mods/outerwildskoreantranslation/) and [Outer Wilds Traditional Chinese Translation](https://outerwildsmods.com/mods/outerwildstraditionalchinesetranslation/).

## Getting started

- Open up the following files in a text editor (I recommend Notepad++ for its easy find and replace tool):

```
TranslationTemplate.sln
manifest.json
TextTranslationPatches.cs
TranslationTemplate.cs
TranslationTemplate.csproj
TranslationTemplate.csproj.user
```

- Open up the "Find in Files" menu (you can do this with Ctrl+Shift+F) and click on the "Replace" tab. When applying changes, be sure to click "Replace All in All Opened Documents" and not just "Replace All".
  - Now find `TranslationTemplate` and replace it with the name of your mod (ex, `CzechTranslation`, `ArabicTranslation`).
  - Next up find "xen" and replace it with your username.
- Open up `manifest.json` and replace "Mod Template" with the readable name of your mod (ex, "Czech Translation", "Arabic Translation").
- For all the files above (except `manifest.json`) change their names to replace "TranslationTemplate" with the name of your mod.

## Translating

Open up `Translations.xml`.

In this file you'll find dictionaries of `key` and `value` pairs. When translating, __only replace what is inside the `value` tags__. The key is what the game uses to look up that value, and must remain in the original English (except for the UI translations near the bottom of the file where the keys are just numbers).

For example, the first entry in the default `Translations.xml` looks like this:

![First entry with key and value both in English](https://user-images.githubusercontent.com/22628069/180037353-1dbff04c-faa9-4dba-9d08-083c75071e0d.png)

In [shippy's Czech translation mod](https://outerwildsmods.com/mods/czechlocalization/) the first entry looks like this:

![First entry with key in English and value in Czech](https://user-images.githubusercontent.com/22628069/180037136-7cb78de8-2a9b-420e-acec-702633f5fbdc.png)

Only translate the value not the key.

You will want to use a program such as Visual Studio Code when editing the xml, because it will flag any mistakes you might make in the syntax. If you accidentally delete a `>` character or remove the end of a `<value>` tag you'll break the translation loading.
