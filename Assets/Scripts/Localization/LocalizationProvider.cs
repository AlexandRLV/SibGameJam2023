using System.Collections.Generic;
using Common.DI;
using LocalMessages;
using LocalMessages.MessageTypes;
using UnityEngine;

namespace Localization
{
    public class LocalizationProvider
    {
        public LocalizationLanguage CurrentLanguage { get; private set; }

        [Inject] private LocalMessageBroker _messageBroker;
        
        private Dictionary<LocalizationLanguage, Dictionary<string, string>> _localizationKeys;
        private Dictionary<string, string> _currentLocalization;

        public void ReadData(LocalizationData data)
        {
            _localizationKeys = new Dictionary<LocalizationLanguage, Dictionary<string, string>>();
            foreach (var container in data.data)
            {
                var localization = new Dictionary<string, string>();
                foreach (var value in container.values)
                {
                    localization.Add(value.key, value.value);
                }
                _localizationKeys.Add(container.language, localization);
            }
        }

        public void SetLanguage(LocalizationLanguage language)
        {
            if (!_localizationKeys.TryGetValue(language, out var localization))
            {
                Debug.LogError($"Невозможно установить текущий язык {language} - в локализации нет такого языка!");
                return;
            }

            _currentLocalization = localization;
            CurrentLanguage = language;

            var message = new LocalizationChangedMessage();
            _messageBroker.Trigger(ref message);
        }

        public string GetLocalization(string key)
        {
            if (_currentLocalization == null)
            {
                Debug.LogError($"Невозможно получить локализацию для ключа {key}! Текущий язык не установлен!");
                return key;
            }

            if (!_currentLocalization.ContainsKey(key))
            {
                Debug.LogError($"Невозможно получить локализацию для ключа {key}! Ключ не найден в таблице!");
                return key;
            }

            return _currentLocalization[key];
        }
    }
}