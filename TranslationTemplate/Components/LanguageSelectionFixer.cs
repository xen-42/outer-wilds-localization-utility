using System.Linq;
using UnityEngine;

namespace TranslationTemplate.Components
{
    public class LanguageSelectionFixer : MonoBehaviour
    {
        private OptionsSelectorElement _selector;
        private string[] _originalList;
        private string[] _customLanguages;
        private Font _originalFont;

        public void Awake()
        {
            _selector = GetComponent<OptionsSelectorElement>();
            _originalList = _selector._optionsList;

            _customLanguages = TranslationTemplate.GetRegisteredLanguages();

            _selector.OnValueChanged += OnValueChanged;
        }

        private void OnEnable()
        {
            _selector._optionsList = _originalList.Concat(_customLanguages).ToArray();

            FixFont(_selector._value);
        }

        private void OnValueChanged(int value)
        {
            FixFont(value);
        }

        private void FixFont(int value)
        {
            var newLang = (TextTranslation.Language)value;
            var newFont = TranslationTemplate.GetLanguage(newLang)?.Font;
            if (newFont != null)
            {
                // The language name can surely be displayed in its own font
                _selector._displayText.font = newFont;
            }
            else
            {
                if (TranslationTemplate.IsCustomLanguage(newLang))
                {
                    // If you have no font probably its just latin alphabet
                    _selector._displayText.font = TextTranslation.s_theTable.m_fonts[(int)TextTranslation.Language.ENGLISH];
                }
                else
                {
                    // This is the one it uses for the default languages
                    _selector._displayText.font = TextTranslation.s_theTable.m_languageFonts[(int)TextTranslation.Language.ENGLISH];
                }
            }
        }
    }
}
