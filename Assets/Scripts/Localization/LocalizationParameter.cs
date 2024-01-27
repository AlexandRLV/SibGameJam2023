using System;
using UnityEngine;

namespace Localization
{
    [Serializable]
    public class LocalizationParameter
    {
        [SerializeField] public string key;
        [SerializeField] public string value;
    }
}