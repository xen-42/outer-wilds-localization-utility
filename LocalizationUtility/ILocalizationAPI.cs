﻿using OWML.ModHelper;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LocalizationUtility
{
	public interface ILocalizationAPI
	{
		#region Add new language
		void RegisterLanguage(ModBehaviour mod, string languageName, string translationPath);
		void RegisterLanguage(ModBehaviour mod, string languageName, string translationPath, string languageToReplace);
		void AddLanguageFont(ModBehaviour mod, string languageName, string assetBundlePath, string fontPath, out Font font);
		void AddLanguageFixer(string languageName, Func<string, string> fixer);
		void SetLanguageDefaultFontSpacing(string languageName, float defaultFontSpacing);
		void SetLanguageFontSizeModifier(string languageName, float fontSizeModifier);
		#endregion

		#region	Add translations to new/existing languages
		void AddTranslation(ModBehaviour mod, string languageName, string translationPath);
		void AddTranslation(string languageName, KeyValuePair<string, string>[] regularEntries, KeyValuePair<string, string>[] shipLogEntries, KeyValuePair<int, string>[] uiEntries);

		void AddRegularTranslation(string languageName, string key, string value);
		void AddRegularTranslation(string languageName, string commonKeyPrefix, params string[] entries);
		void AddRegularTranslation(string languageName, params KeyValuePair<string, string>[] entries);

		void AddShiplogTranslation(string languageName, string key, string value);
		void AddShiplogTranslation(string languageName, string commonKeyPrefix, params string[] entries);
		void AddShiplogTranslation(string languageName, params KeyValuePair<string, string>[] entries);

		void AddUITranslation(string languageName, int key, string value);
		void AddUITranslation(string languageName, params KeyValuePair<int, string>[] entries);
		#endregion


		#region obsolete
		[Obsolete] void AddLanguageFont(ModBehaviour mod, string languageName, string assetBundlePath, string fontPath);
		#endregion
	}
}
