using UnityEngine;

namespace GameCore.Levels
{
    [CreateAssetMenu(menuName = "Configs/Levels Data")]
    public class LevelsData : ScriptableObject
    {
        [SerializeField] public LevelInfo multiplayerLevel; 
        [SerializeField] public LevelInfo[] levels;

        public void Initialize()
        {
            for (int i = 0; i < levels.Length; i++)
            {
                var level = levels[i];
                level.id = i;
            }
        }
    }
}