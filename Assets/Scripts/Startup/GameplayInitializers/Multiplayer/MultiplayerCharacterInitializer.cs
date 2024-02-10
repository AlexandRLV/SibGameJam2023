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
        public override void Initialize()
        {
            var spawns = GameContainer.InGame.Resolve<PlayerSpawns>();

            var thinMouseVisualsPrefab = Resources.Load<CharacterVisuals>("Prefabs/CharacterVisuals/ThinMouseVisuals");
            var thinMouseVisuals = Object.Instantiate(thinMouseVisualsPrefab);

            var fatMouseVisualsPrefab = Resources.Load<CharacterVisuals>("Prefabs/CharacterVisuals/FatMouseVisuals");
            var fatMouseVisuals = Object.Instantiate(fatMouseVisualsPrefab);

            var localPlayerPrefab = Resources.Load<OneMouseLocalPlayer>("Prefabs/OneMousePlayer");
            var localPlayer = GameContainer.InstantiateAndResolve(localPlayerPrefab);

            var remotePlayerPrefab = Resources.Load<RemotePlayer>("Prefabs/RemotePlayer");
            var remotePlayer = GameContainer.InstantiateAndResolve(remotePlayerPrefab);

            bool isMaster = GameContainer.Common.Resolve<GameClientData>().IsMaster;
            if (isMaster)
            {
                var fatMouseMovementPrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/FatMouseCharacter");
                var fatMouseMovement = GameContainer.InstantiateAndResolve(fatMouseMovementPrefab);
                fatMouseMovement.transform.SetPositionAndRotation(spawns.FatSpawn.position, spawns.FatSpawn.rotation);
                fatMouseMovement.Initialize(fatMouseVisuals);
                localPlayer.Initialize(fatMouseMovement, PlayerMouseType.FatMouse);
                
                remotePlayer.Initialize(thinMouseVisuals);
            }
            else
            {
                var littleMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/ThinMouseCharacter");
                var thinMouseMovement = GameContainer.InstantiateAndResolve(littleMousePrefab);
                thinMouseMovement.transform.SetPositionAndRotation(spawns.ThinSpawn.position, spawns.ThinSpawn.rotation);
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