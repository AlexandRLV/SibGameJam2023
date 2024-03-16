using GameCore.Enemies.NewEnemy.Parameters;
using GameCore.Enemies.RouteControl;
using UnityEngine;

namespace GameCore.Enemies.NewEnemy.Spawner
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] public Transform spawnPoint;
        [SerializeField] public EnemyMainPreset spawnPreset;
        [SerializeField] public Waypoint[] waypoints;
    }
}