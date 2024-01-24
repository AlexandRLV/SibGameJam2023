using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
    [Serializable]
    public class LocalizationDataContainer
    {
        [SerializeField] public LocalizationLanguage language;
        [SerializeField] public List<LanguageContainer> values;
    }

    [Serializable]
    public class LanguageContainer
    {
        [SerializeField] public string key;
        [SerializeField] public string value;
    }
    
    [CreateAssetMenu(menuName = "Localization/Localization Data")]
    public class LocalizationData : ScriptableObject
    {
        [SerializeField] public List<LocalizationDataContainer> data;
    }
}