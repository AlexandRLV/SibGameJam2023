using System;
using UnityEngine;

namespace GameCore.Levels
{
    [Serializable]
    public class LevelInfo
    {
        [NonSerialized] public int id;
        [SerializeField] public bool hasIntro;
        [SerializeField] public string sceneName;
        [SerializeField] public int cheeseCount;
        [SerializeField] public string levelName;
        [SerializeField] public string[] hints;
    }
}