using UnityEngine;

namespace GameCore.Enemies.NewEnemy.Parameters
{
    [CreateAssetMenu(menuName = "Configs/Enemies/View Preset")]
    public class EnemyViewPreset : ScriptableObject
    {
        [SerializeField] public float autoDetectRadius;
        [SerializeField] public float viewAngle;
        [SerializeField] public float viewDistance;
        [SerializeField] public float viewStartRadius;
        [SerializeField] public float viewStartOffset;
        
        [SerializeField] public Color normalConeColor;
        [SerializeField] public Color alertConeColor;
    }
}