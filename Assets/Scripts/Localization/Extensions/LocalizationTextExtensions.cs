using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Localization.Extensions
{
    public static class LocalizationTextExtensions
    {
        private static readonly StringBuilder _resultStringBuilder = new();
        private static readonly StringBuilder _parameterStringBuilder = new();
        
        public static string GetTextLocalization(this LocalizationProvider provider, string key)
        {
            if (provider.currentTextLocalization == null)
            {
                Debug.LogError($"Невозможно получить локализацию для ключа {key}! Текущий язык не установлен!");
                return key;
            }

            if (!provider.currentTextLocalization.ContainsKey(key))
            {
                Debug.LogError($"Невозможно получить локализацию для ключа {key}! Ключ не найден в таблице!");
                return key;
            }

            return provider.currentTextLocalization[key];
        }

        public static string GetTextLocalization(this LocalizationProvider provider, string key, List<LocalizationParameter> parameters)
        {
            if (parameters == null)
                return GetTextLocalization(provider, key);
            
            if (provider.currentTextLocalization == null)
            {
                Debug.LogError($"Невозможно получить локализацию для ключа {key}! Текущий язык не установлен!");
                return key;
            }

            if (!provider.currentTextLocalization.ContainsKey(key))
            {
                Debug.LogError($"Невозможно получить локализацию для ключа {key}! Ключ не найден в таблице!");
                return key;
            }

            _resultStringBuilder.Clear();
            _resultStringBuilder.Append(provider.currentTextLocalization[key]); 
            
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