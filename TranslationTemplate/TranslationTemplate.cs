using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TranslationTemplate.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TranslationTemplate
{
    public class TranslationTemplate : ModBehaviour
    {
        public static TranslationTemplate Instance;

        public static TextTranslation.Language currentLanguage = TextTranslation.Language.UNKNOWN;

        private Dictionary<TextTranslation.Language, CustomLanguage> _customLanguages = new();

        public static bool IsCustomLanguage(TextTranslation.Language language)
        {
            return language > TextTranslation.Language.TURKISH;
        }

        public static CustomLanguage GetLanguage(TextTranslation.Language lang)
        {
            if (Instance._customLanguages.TryGetValue(lang, out var customLanguage))
            {
                return customLanguage;
            }
            return null;
        }

        public static CustomLanguage GetLanguage()
        {
            return GetLanguage(currentLanguage);
        }

        public static string[] GetRegisteredLanguages()
        {
            return Instance._customLanguages.Values.Select(x => x.Name).ToArray();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            ModHelper.Menus.MainMenu.OnInit += InitTitleMenu;

            WriteLine($"Translation mod {nameof(TranslationTemplate)} is loaded!");

            var czech = new CustomLanguage("český", "assets/czech.xml", this);
            _customLanguages.Add((TextTranslation.Language)12, czech);
        }

        private void OnDestroy()
        {
            ModHelper.Menus.MainMenu.OnInit -= InitTitleMenu;
        }

        public void InitTitleMenu()
        {
            var options = GameObject.Find("TitleMenu/OptionsCanvas/OptionsMenu-Panel/OptionsDisplayPanel/TextAudioMenu/Content/UIElement-Language");
            options.AddComponent<LanguageSelectionFixer>();
        }

        public static void WriteLine(string msg, MessageType type = MessageType.Info)
        {
            Instance.ModHelper.Console.WriteLine($"{type}: {msg}", type);
        }

        public static void WriteError(string msg) => WriteLine(msg, MessageType.Error);
    }
}
