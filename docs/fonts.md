# Fonts

## Default fonts

Fonts in current use by the Latin localizations of the game:

- Translator: [Space Mono](https://fonts.google.com/specimen/Space+Mono) (monospaced)
- Conversations with Hearthians: [Brandon Grotesque Black](https://fonts.adobe.com/fonts/brandon-grotesque)
- Ship display: `VCR_OSD_MONO`
- UI: _some sans-serif font_
- Title screen / Endgame: _some sans-serif font_

These fonts have partial support for non-English characters, but depending on how different your language's writing system is, you may need to supply a custom font.

## Custom fonts

You can use this tool to add custom fonts to the game. **This will patch all the fonts in the game with this single one.**

### Limitations

Currently, patching the font in has the two following unintended effects:

1. Ship signalscope font becomes tiny
2. Ship display font becomes disabled

### How to make an asset bundle with a font and avoid going insane

1. Install Unity Hub
2. Go to the list of Unity editor versions and download 2019.4.27f (pick the Unity Hub version)
3. Create a new Unity project (from any template). If this results in an open project in the Unity editor, close it for now.
4. Into the `Assets/` subfolder of the new Unity project, drag the font files you've selected (e.g. from Google Fonts), **before opening the Unity editor**. (The Unity editor will create a bunch of `.meta` files that will allow the files within the folder to be used inside an asset bundle, but it only appears to do this at launch.)
5. Open the project in the Unity editor.
6. In the Package Manager (available through one of the menus), find and enable Asset Bundle Browser.
7. Open it via Window > AssetBundle Browser.
8. On the **Configure** tab, drag from within the Unity editor the font file you wish to bundle. (Don't drag it from Windows Explorer; don't try adding via Inspect tab and Add file/folder.)
9. Switch to **Build** tab and click "Build".
10. Grab the newly created asset bundle (and its `.manifest` file) and copy it to your mod directory.
11. Use the `api.AddLanguageFont` method, listing (1) the path to the asset bundle, (2) the path within the asset bundle to the font. (You can get this from the manifest file!)
12. Change the `.csproj` file to include the asset bundles. (Current settings should already do this, so long as they're saved under `assets/`.)