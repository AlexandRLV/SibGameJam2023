using GameCore.Enemies.EnemyObject;
using GameCore.Player;
using UnityEngine;

namespace GameCore.Enemies.NewEnemy.Spawner
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemySpawnPoint[] _spawnPoints;

        public void Spawn(IPlayer player, NewEnemyMovement movementPrefab)
        {
            foreach (var spawnPoint in _spawnPoints)
            {
                var spawnTransform = spawnPoint.transform;
                var enemy = Instantiate(movementPrefab, spawnTransform.position, spawnTransform.rotation);
                var visuals = Instantiate(spawnPoint.spawnPreset.enemyVisuals);
                enemy.Initialize(spawnPoint.spawnPreset, visuals, player);
                
                if (spawnPoint.spawnPreset.movementType != EnemyMovementType.NoWalk && spawnPoint.waypoints is { Length: > 0 })
                    enemy.SetWaypointsMovement(spawnPoint.waypoints);
            }
        }
    }
}