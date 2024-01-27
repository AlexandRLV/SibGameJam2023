using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
    [Serializable]
    public class LocalizationLanguageConfig
    {
        [SerializeField] public SystemLanguage language;
        [SerializeField] public int columnId;
    }
    
    [CreateAssetMenu(menuName = "Localization/Localization Config")]
    public class LocalizationConfig : ScriptableObject
    {
        [SerializeField] public List<LocalizationLanguageConfig> languages;
    }
}