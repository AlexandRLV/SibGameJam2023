using UnityEngine;

namespace GameCore.Enemies.NewEnemy.Parameters
{
    [CreateAssetMenu(menuName = "Configs/Enemies/Timers")]
    public class EnemyTimersParameters : ScriptableObject
    {
        [SerializeField] public float disableProvokedSeconds;
        [SerializeField] public float triggerAlarmSeconds;
    }
}