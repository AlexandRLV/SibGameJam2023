using System.Collections.Generic;
using System.Text;
using Common.DI;
using LocalMessages;
using LocalMessages.MessageTypes;
using UnityEngine;

namespace Localization
{
    public class LocalizationProvider
    {
        public SystemLanguage CurrentLanguage { get; private set; }

        [Inject] private LocalMessageBroker _messageBroker;

        private StringBuilder _resultStringBuilder;
        private StringBuilder _parameterStringBuilder;
        private Dictionary<SystemLanguage, Dictionary<string, string>> _localizationKeys;
        private Dictionary<string, string> _currentLocalization;

        public void ReadData(LocalizationData data)
        {
            _resultStringBuilder = new StringBuilder();
            _parameterStringBuilder = new StringBuilder();
            
            _localizationKeys = new Dictionary<SystemLanguage, Dictionary<string, string>>();
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

        public bool HasLanguage(SystemLanguage language) => _localizationKeys.ContainsKey(language);

        public void SetLanguage(SystemLanguage language)
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

        public string GetLocalization(string key, List<LocalizationParameter> parameters)
        {
            if (parameters == null)
                return GetLocalization(key);
            
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

            _resultStringBuilder.Clear();
            _resultStringBuilder.Append(_currentLocalization[key]); 
            
            _parameterStringBuilder.Clear();
            foreach (var parameter in parameters)
            {
                _parameterStringBuilder.Append("[{");
                _parameterStringBuilder.Append(parameter.key);
                _parameterStringBuilder.Append("}]");

                _resultStringBuilder.Replace(_parameterStringBuilder.ToString(), parameter.value);
            }

            return _resultStringBuilder.ToString();
        }
    }
}