using System;
using GameCore.Character.Animation;
using GameCore.Enemies.EnemyObject;
using UnityEngine;

namespace GameCore.Enemies.NewEnemy.Parameters
{
    [CreateAssetMenu(menuName = "Configs/Enemies/Main Preset")]
    public class EnemyMainPreset : ScriptableObject
    {
        [NonSerialized] public bool hasHeadRotationPreset;
        
        [SerializeField] public float walkSpeed;
        [SerializeField] public float stoppingDistance;
        [SerializeField] public EnemyMovementType movementType;
        [SerializeField] public CharacterVisuals enemyVisuals;
        
        [Header("Other presets")]
        [SerializeField] public EnemyViewPreset viewPreset;
        [SerializeField] public EnemyHeadRotationPreset headRotationPreset;
    }
}