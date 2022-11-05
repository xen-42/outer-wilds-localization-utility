using HarmonyLib;
using OWML.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace LocalizationUtility
{
    [HarmonyPatch]
    public static class SliderPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.Init))]
        private static void PlayerData_Init(SettingsSave settingsData)
        {
            LanguageSaveFile save = LocalizationUtility.Load();
            // Check for saved language
            if (save != null && !string.IsNullOrWhiteSpace(save.language))
            {
                CustomLanguage customLanguage = LocalizationUtility.Instance.GetLanguage(save.language);
                if (customLanguage != null) settingsData.language = customLanguage.Language;
            }
            // Set to the first custom language
            else if (LocalizationUtility.Instance.HasAnyCustomLanguages)
                settingsData.language = LocalizationUtility.Instance._customLanguages.Values.FirstOrDefault((pair)=>pair.IsCustom).Language;
        }

        /// <summary>
        /// Avoid save breaking
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.SaveSettings))]
        private static void PlayerData_SaveSettings_Prefix()
        {
            if (LocalizationUtility.IsVanillaLanguage(PlayerData._settingsSave.language))
            {
                LocalizationUtility.Save(new LanguageSaveFile(string.Empty));
                return;
            }

            CustomLanguage customLanguage = LocalizationUtility.Instance.GetLanguage(PlayerData._settingsSave.language);
            if (customLanguage != null)
            {
                PlayerData._settingsSave.language = customLanguage.LanguageToReplace;
                LocalizationUtility.Save(new LanguageSaveFile(customLanguage.Name));
            }
            else
                LocalizationUtility.Save(new LanguageSaveFile(string.Empty));
        }

        /// <summary>
        /// Set back to saved custom language
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.SaveSettings))]
        private static void PlayerData_SaveSettings_Postfix()
        {
            LanguageSaveFile save = LocalizationUtility.Load();
            if (save != null && !string.IsNullOrEmpty(save.language))
            {
                CustomLanguage customLanguage = LocalizationUtility.Instance.GetLanguage(save.language);
                if (customLanguage != null) PlayerData._settingsSave.language = customLanguage.Language;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.GetSavedLanguage))]
        private static bool PlayerData_GetSavedLanguage(ref TextTranslation.Language __result)
        {
            LanguageSaveFile save = LocalizationUtility.Load();
            if (save != null && !string.IsNullOrWhiteSpace(save.language))
            {
                CustomLanguage customLanguage = LocalizationUtility.Instance.GetLanguage(save.language);
                if (customLanguage != null)
                {
                    __result = customLanguage.Language;
                    return false;
                }
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsSave), nameof(SettingsSave.GetLanguageStrings))]
        private static void SettingsSave_GetLanguageStrings(ref string[] __result)
        {
            if (LocalizationUtility.Instance.HasAnyCustomLanguages)
            {
                Array.Resize(ref __result, (int)EnumUtils.GetMaxValue<TextTranslation.Language>() + 1);
                __result[(int)TextTranslation.Language.TOTAL] = "Total";
                foreach (var profile in LocalizationUtility.Instance._customLanguages.Values)
                {
                    if (profile.IsCustom)
                    {
                        __result[(int)profile.Language] = profile.Name;
                    }
                }
            }
        }

        /// <summary>
        /// For skipping <see cref="TextTranslation.Language.TOTAL"/>
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OptionsSelectorElement), nameof(OptionsSelectorElement.OptionsMove))]
        private static void OptionsSelectorElement_OptionsMove(OptionsSelectorElement __instance, Vector2 moveVector)
        {
            if (__instance._settingId == SettingsID.LANGUAGE)
            {
                bool increasing = __instance._directionality == OptionsSelectorElement.Direction.HORIZONTAL ? moveVector.x > 0 : (__instance._directionality == OptionsSelectorElement.Direction.VERTICAL ? moveVector.y < 0 : false);
                bool decreasing = __instance._directionality == OptionsSelectorElement.Direction.HORIZONTAL ? moveVector.x < 0 : (__instance._directionality == OptionsSelectorElement.Direction.VERTICAL ? moveVector.y > 0 : false);
                if ((increasing && __instance._value == (int)TextTranslation.Language.TOTAL - 1) || (decreasing && __instance._value == (int)TextTranslation.Language.TOTAL + 1))
                {
                    __instance._value = (int)TextTranslation.Language.TOTAL;
                }
            }
        }
    }

    public class LanguageSaveFile
    {
        public readonly string language;
        public LanguageSaveFile(string language) => this.language = language;

        public static readonly LanguageSaveFile DEFAULT = new LanguageSaveFile(string.Empty);
    }
}
