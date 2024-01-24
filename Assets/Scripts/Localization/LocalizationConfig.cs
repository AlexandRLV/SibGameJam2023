using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
    [Serializable]
    public class LocalizationLanguageConfig
    {
        [SerializeField] public LocalizationLanguage language;
        [SerializeField] public int columnId;
    }
    
    [CreateAssetMenu(menuName = "Localization/Localization Config")]
    public class LocalizationConfig : ScriptableObject
    {
        // TODO: change to settings and country determine on start
        [SerializeField] public LocalizationLanguage defaultLanguage;
        [SerializeField] public List<LocalizationLanguageConfig> languages;
    }
}