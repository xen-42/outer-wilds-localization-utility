using HarmonyLib;
using UnityEngine;

namespace LocalizationUtility
{
    [HarmonyPatch]
    public static class FontPatches
    {
        private static bool UsingCustomFont()
        {
            if (TextTranslation.s_theTable.m_language != LocalizationUtility.languageToReplace) return false;
            return LocalizationUtility.Instance.GetLanguage()?.Font != null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetFont))]
        public static bool TextTranslation_GetFont(ref Font __result)
        {
            if (!UsingCustomFont()) return true;

            __result = LocalizationUtility.Instance.GetLanguage().Font;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.IsLanguageLatin))]
        public static bool TextTranslation_IsLanguageLatin(ref bool __result)
        {
            if (TextTranslation.s_theTable.m_language != LocalizationUtility.languageToReplace) return true;

            if (UsingCustomFont())
            {
                __result = false;
                return false;
            }
            else 
            {
                __result = true;
                return false;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TranslatedSign), nameof(TranslatedSign.Start))]
        public static void TranslatedSign_Start(TranslatedSign __instance)
        {
            if (!UsingCustomFont()) return;

            __instance._interactVolume.gameObject.SetActive(true);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetLanguageFont))]
        public static bool TextTranslation_GetLanguageFont(ref Font __result)
        {
            if (!UsingCustomFont()) return true;

            __result = LocalizationUtility.Instance.GetLanguage().Font;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NomaiTranslatorProp), nameof(NomaiTranslatorProp.InitializeFont))]
        public static bool NomaiTranslatorProp_InitializeFont(NomaiTranslatorProp __instance)
        {
            if (!UsingCustomFont()) return true;

            __instance._fontInUse = LocalizationUtility.Instance.GetLanguage().Font;
            __instance._dynamicFontInUse = LocalizationUtility.Instance.GetLanguage().Font;
            __instance._textField.font = LocalizationUtility.Instance.GetLanguage().Font;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStyleManager), nameof(UIStyleManager.GetShipLogFont))]
        public static bool UIStyleManager_GetShipLogFont(ref Font __result)
        {
            if (!UsingCustomFont()) return true;

            __result = LocalizationUtility.Instance.GetLanguage().Font;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameOverController), nameof(GameOverController.SetupGameOverScreen))]
        public static bool GameOverController_SetupGameOverScreen(GameOverController __instance)
        {
            if (!UsingCustomFont()) return true;

            __instance._deathText.font = LocalizationUtility.Instance.GetLanguage().Font;

            return false;
        }
    }
}
