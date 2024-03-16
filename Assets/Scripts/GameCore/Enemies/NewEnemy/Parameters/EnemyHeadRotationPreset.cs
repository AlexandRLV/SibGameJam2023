using UnityEngine;

namespace GameCore.Enemies.NewEnemy.Parameters
{
    [CreateAssetMenu(menuName = "Configs/Enemies/Head Rotation Preset")]
    public class EnemyHeadRotationPreset : ScriptableObject
    {
        [SerializeField] public float headRotationAngle;
        [SerializeField] public float headRotationTime;
        [SerializeField] public float headRotationInterval;
        [SerializeField] public AnimationCurve headRotationCurve;
    }
}