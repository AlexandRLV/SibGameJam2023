using UnityEngine;

namespace GameCore.RoundControl
{
    [CreateAssetMenu(menuName = "Configs/Round Settings")]
    public class RoundSettings : ScriptableObject
    {
        public float RoundLengthSeconds => roundLengthMinutes * 60f;

        [SerializeField] public int roundLengthMinutes;
        [SerializeField] public float playerDetectedToLoseSeconds;
    }
}