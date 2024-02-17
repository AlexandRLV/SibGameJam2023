using Common.DI;
using GameCore;
using GameCore.Character.Animation;
using GameCore.Character.Movement;
using GameCore.Player;
using GameCore.Player.Network;
using Networking;
using UnityEngine;

namespace Startup.GameplayInitializers.Multiplayer
{
    public class MultiplayerCharacterInitializer : InitializerBase
    {
        [SerializeField] private CharacterVisuals _thinMouseVisuals;
        [SerializeField] private CharacterVisuals _fatMouseVisuals;
        [SerializeField] private CharacterMovement _thinMovement;
        [SerializeField] private CharacterMovement _fatMovement;
        [SerializeField] private PlayerSpawns _playerSpawns;
        [SerializeField] private OneMouseLocalPlayer _oneMouseLocalPlayer;
        [SerializeField] private RemotePlayer _remotePlayer;
        
        public override void Initialize()
        {
            var thinMouseVisuals = Instantiate(_thinMouseVisuals);
            var fatMouseVisuals = Instantiate(_fatMouseVisuals);

            var localPlayer = GameContainer.InstantiateAndResolve(_oneMouseLocalPlayer);
            var remotePlayer = GameContainer.InstantiateAndResolve(_remotePlayer);

            bool isMaster = GameContainer.Common.Resolve<GameClientData>().IsMaster;
            if (isMaster)
            {
                var fatMouseMovement = GameContainer.InstantiateAndResolve(_fatMovement);
                fatMouseMovement.transform.SetPositionAndRotation(_playerSpawns.FatSpawn.position, _playerSpawns.FatSpawn.rotation);
                fatMouseMovement.Initialize(fatMouseVisuals);
                localPlayer.Initialize(fatMouseMovement, PlayerMouseType.FatMouse);
                
                remotePlayer.Initialize(thinMouseVisuals);
            }
            else
            {
                var thinMouseMovement = GameContainer.InstantiateAndResolve(_thinMovement);
                thinMouseMovement.transform.SetPositionAndRotation(_playerSpawns.ThinSpawn.position, _playerSpawns.ThinSpawn.rotation);
                thinMouseMovement.Initialize(thinMouseVisuals);
                localPlayer.Initialize(thinMouseMovement, PlayerMouseType.ThinMouse);
                
                remotePlayer.Initialize(fatMouseVisuals);
            }
            
            GameContainer.InGame.Register<IPlayer>(localPlayer);
            GameContainer.InGame.Register(remotePlayer);
        }

        public override void Dispose()
        {
        }
    }
}