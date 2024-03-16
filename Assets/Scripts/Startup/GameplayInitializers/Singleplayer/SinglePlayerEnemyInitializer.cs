using Common.DI;
using GameCore.Enemies.NewEnemy;
using GameCore.Enemies.NewEnemy.Spawner;
using GameCore.Player;
using UnityEngine;

namespace Startup.GameplayInitializers.Singleplayer
{
    public class SinglePlayerEnemyInitializer : InitializerBase
    {
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private NewEnemyMovement _enemyMovementPrefab;
        
        public override void Initialize()
        {
            var player = GameContainer.InGame.Resolve<IPlayer>();
            _enemySpawner.Spawn(player, _enemyMovementPrefab);
        }

        public override void Dispose()
        {
        }
    }
}