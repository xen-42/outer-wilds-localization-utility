﻿using HarmonyLib;
using UnityEngine;

namespace LocalizationUtility
{
    [HarmonyPatch]
    public static class FontPatches
    {
        private static bool UsingCustomFont()
        {
            return LocalizationUtility.Instance.GetLanguage()?.Font != null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetFont))]
        public static bool TextTranslation_GetFont(ref Font __result)
        {
            if (LocalizationUtility.IsVanillaLanguage(TextTranslation.s_theTable.m_language)) return true;

            __result = UsingCustomFont() ? LocalizationUtility.Instance.GetLanguage().Font : TextTranslation.s_theTable.m_dynamicFonts[(int)TextTranslation.Language.ENGLISH];
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.IsLanguageLatin))]
        public static bool TextTranslation_IsLanguageLatin(ref bool __result)
        {
            if (LocalizationUtility.IsVanillaLanguage(TextTranslation.s_theTable.m_language)) return true;

            __result = !UsingCustomFont();
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TranslatedSign), nameof(TranslatedSign.Start))]
        public static void TranslatedSign_Start(TranslatedSign __instance)
        {
            if (LocalizationUtility.IsVanillaLanguage(TextTranslation.s_theTable.m_language)) return;

            __instance._interactVolume.gameObject.SetActive(true);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetLanguageFont))]
        public static bool TextTranslation_GetLanguageFont(ref Font __result)
        {
            if (LocalizationUtility.IsVanillaLanguage(TextTranslation.s_theTable.m_language))
                __result = TextTranslation.GetFont(true); // Set vanilla language font to dynamic so that custom language's names will actually show up correctly in settings.
            else if (UsingCustomFont())
                __result = LocalizationUtility.Instance.GetLanguage().Font;
            else
                __result = TextTranslation.s_theTable.m_dynamicFonts[(int)TextTranslation.Language.ENGLISH];
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStyleManager), nameof(UIStyleManager.GetMenuFont))]
        public static bool UIStyleManager_GetMenuFont(ref Font __result)
        {
            if (LocalizationUtility.IsVanillaLanguage(PlayerData.GetSavedLanguage())) return true;

            if (!UsingCustomFont()) return true;

            __result = LocalizationUtility.Instance.GetLanguage().Font;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameOverController), nameof(GameOverController.SetupGameOverScreen))]
        public static bool GameOverController_SetupGameOverScreen(GameOverController __instance)
        {
            if (LocalizationUtility.IsVanillaLanguage(TextTranslation.s_theTable.m_language)) return true;

            if (!UsingCustomFont()) return true;

            __instance._deathText.font = LocalizationUtility.Instance.GetLanguage().Font;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetDefaultFontSpacing))]
        public static bool TextTranslation_GetDefaultFontSpacing(ref float __result)
        {
            if (LocalizationUtility.IsVanillaLanguage(TextTranslation.s_theTable.m_language)) return true;

            if (UsingCustomFont()) __result = 1;
            else __result = TextTranslation.s_theTable.m_defaultSpacing[(int)TextTranslation.Language.ENGLISH];

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetFontSizeModifier))]
        public static bool TextTranslation_GetFontSizeModifier(ref float __result)
        {
            if (LocalizationUtility.IsVanillaLanguage(TextTranslation.s_theTable.m_language)) return true;

            if (UsingCustomFont()) __result = 1;
            else __result = TextTranslation.s_theTable.m_fontSizeModifier[(int)TextTranslation.Language.ENGLISH];


            return false;
        }
    }
}