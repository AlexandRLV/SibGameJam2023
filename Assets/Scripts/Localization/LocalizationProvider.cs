using System.Collections.Generic;
using Common.DI;
using LocalMessages;
using LocalMessages.MessageTypes;
using UnityEngine;

namespace Localization
{
    public class LocalizationProvider
    {
        public SystemLanguage CurrentLanguage { get; private set; }
        
        public Dictionary<string, string> currentTextLocalization;
        public Dictionary<string, Sprite> currentImageLocalization;

        [Inject] private LocalMessageBroker _messageBroker;

        private Dictionary<SystemLanguage, Dictionary<string, string>> _textLocalizations;
        private Dictionary<SystemLanguage, Dictionary<string, Sprite>> _imageLocalizations;

        public void ReadData(LocalizationData data)
        {
            _textLocalizations = new Dictionary<SystemLanguage, Dictionary<string, string>>();
            foreach (var container in data.textsData)
            {
                var localization = new Dictionary<string, string>();
                foreach (var value in container.texts)
                {
                    localization.Add(value.key, value.value);
                }
                _textLocalizations.Add(container.language, localization);
            }

            _imageLocalizations = new Dictionary<SystemLanguage, Dictionary<string, Sprite>>();
            foreach (var container in data.imagesData)
            {
                var localization = new Dictionary<string, Sprite>();
                foreach (var value in container.images)
                {
                    localization.Add(value.key, value.value);
                }
                _imageLocalizations.Add(container.language, localization);
            }
        }

        public bool HasLanguage(SystemLanguage language) => _textLocalizations.ContainsKey(language);

        public void SetLanguage(SystemLanguage language)
        {
            if (!_textLocalizations.TryGetValue(language, out var localization))
            {
                Debug.LogError($"Невозможно установить текущий язык {language} - в локализации нет такого языка!");
                return;
            }

            currentTextLocalization = localization;
            if (_imageLocalizations.TryGetValue(language, out var images))
                currentImageLocalization = images;
            
            CurrentLanguage = language;

            var message = new LocalizationChangedMessage();
            _messageBroker.Trigger(ref message);
        }
    }
}