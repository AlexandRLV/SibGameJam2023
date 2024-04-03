using UnityEngine;

namespace Localization.Extensions
{
    public static class LocalizationImageExtensions
    {
        public static bool TryGetImageLocalization(this LocalizationProvider provider, string key, out Sprite value)
        {
            value = null;
            if (provider.currentImageLocalization == null)
            {
                Debug.LogError($"Невозможно получить локализацию для ключа {key}! Текущий язык не установлен!");
                return false;
            }

            if (!provider.currentImageLocalization.ContainsKey(key))
            {
                Debug.LogError($"Невозможно получить локализацию для ключа {key}! Ключ не найден в таблице!");
                return false;
            }

            value = provider.currentImageLocalization[key];
            return true;
        }
    }
}