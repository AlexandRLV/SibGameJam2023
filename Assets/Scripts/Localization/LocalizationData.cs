using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
    [Serializable]
    public class LocalizationTextDataContainer
    {
        [SerializeField] public SystemLanguage language;
        [SerializeField] public List<LanguageTextContainer> texts;
    }

    [Serializable]
    public class LanguageTextContainer
    {
        [SerializeField] public string key;
        [SerializeField] public string value;
    }
    
    [Serializable]
    public class LocalizationImageDataContainer
    {
        [SerializeField] public SystemLanguage language;
        [SerializeField] public List<LanguageImageContainer> images;
    }

    [Serializable]
    public class LanguageImageContainer
    {
        [SerializeField] public string key;
        [SerializeField] public Sprite value;
    }
    
    [CreateAssetMenu(menuName = "Configs/Localization/Localization Data")]
    public class LocalizationData : ScriptableObject
    {
        [SerializeField] public List<LocalizationTextDataContainer> textsData;
        [SerializeField] public List<LocalizationImageDataContainer> imagesData;
    }
}