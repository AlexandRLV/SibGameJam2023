using Common.DI;
using GameCore;
using GameCore.Character.Animation;
using GameCore.Character.Movement;
using GameCore.Player;
using UnityEngine;

namespace Startup.GameplayInitializers.Singleplayer
{
    public class SinglePlayerCharacterInitializer : InitializerBase
    {
        [SerializeField] private CharacterVisuals _thinMouseVisuals;
        [SerializeField] private CharacterVisuals _fatMouseVisuals;
        [SerializeField] private CharacterMovement _thinMovement;
        [SerializeField] private CharacterMovement _fatMovement;
        [SerializeField] private PlayerSpawns _playerSpawns;
        
        public override void Initialize()
        {
            var thinMouseVisuals = Instantiate(_thinMouseVisuals);

            var thinMouseMovement = GameContainer.InstantiateAndResolve(_thinMovement);
            thinMouseMovement.transform.SetPositionAndRotation(_playerSpawns.ThinSpawn.position, _playerSpawns.ThinSpawn.rotation);
            thinMouseMovement.Initialize(thinMouseVisuals);

            var fatMouseVisuals = Instantiate(_fatMouseVisuals);
            var fatMouseMovement = GameContainer.InstantiateAndResolve(_fatMovement);
            fatMouseMovement.transform.SetPositionAndRotation(_playerSpawns.FatSpawn.position, _playerSpawns.FatSpawn.rotation);
            fatMouseMovement.Initialize(fatMouseVisuals);

            Physics.IgnoreCollision(thinMouseMovement.Collider, fatMouseMovement.Collider);

            var player = GameContainer.CreateGameObjectWithComponent<TwoMousePlayer>("TwoMousePlayer");
            player.Initialize(fatMouseMovement, thinMouseMovement);
            GameContainer.InGame.Register<IPlayer>(player);
        }

        public override void Dispose()
        {
        }
    }
}