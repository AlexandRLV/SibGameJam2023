using UnityEngine;

namespace GameCore.Player.Network
{
    [CreateAssetMenu(menuName = "Configs/Network Movement Parameters")]
    public class NetworkParameters : ScriptableObject
    {
        [SerializeField] public int SendTickUpdateRate;
        [SerializeField] public int SendPositionUpdateRate;
        [SerializeField] public int InterpolationTickDelay = 10;
        [SerializeField] public int TickDivergenceTolerance = 1;
        [SerializeField] [Range(1f, 3f)] public float MaxExtrapolationTimePercent;
        [SerializeField] public float MovementThreshold;
        [SerializeField] public float RotationAngleThreshold;
    }
}