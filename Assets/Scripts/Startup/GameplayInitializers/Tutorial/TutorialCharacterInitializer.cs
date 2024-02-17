using Common.DI;
using GameCore;
using GameCore.Character.Animation;
using GameCore.Character.Movement;
using GameCore.Player;
using UnityEngine;

namespace Startup.GameplayInitializers.Tutorial
{
    public class TutorialCharacterInitializer : InitializerBase
    {
        [SerializeField] private CharacterVisuals _fatMouseVisuals;
        [SerializeField] private CharacterMovement _fatMouseMovement;

        [Inject] private PlayerSpawns _playerSpawns;
        
        public override void Initialize()
        {
            var fatMouseVisuals = Instantiate(_fatMouseVisuals);
            var fatMouseMovement = GameContainer.InstantiateAndResolve(_fatMouseMovement);
            fatMouseMovement.transform.SetPositionAndRotation(_playerSpawns.FatSpawn.position, _playerSpawns.FatSpawn.rotation);
            fatMouseMovement.Initialize(fatMouseVisuals);

            var player = GameContainer.CreateGameObjectWithComponent<OneMouseSinglePlayer>("OneMousePlayer");
            player.Initialize(fatMouseMovement, PlayerMouseType.FatMouse);
            
            GameContainer.InGame.Register<IPlayer>(player);
        }

        public override void Dispose()
        {
        }
    }
}