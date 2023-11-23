using System.Collections;
using Common;
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
    public class MultiplayerCharacterInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var spawns = GameContainer.InGame.Resolve<PlayerSpawns>();

            var thinMouseVisualsPrefab = Resources.Load<CharacterVisuals>("Prefabs/CharacterVisuals/ThinMouseVisuals");
            var thinMouseVisuals = Object.Instantiate(thinMouseVisualsPrefab);

            var fatMouseVisualsPrefab = Resources.Load<CharacterVisuals>("Prefabs/CharacterVisuals/FatMouseVisuals");
            var fatMouseVisuals = Object.Instantiate(fatMouseVisualsPrefab);

            var localPlayerPrefab = Resources.Load<OneMouseLocalPlayer>("Prefabs/OneMousePlayer");
            var localPlayer = Object.Instantiate(localPlayerPrefab);

            var remotePlayerPrefab = Resources.Load<RemotePlayer>("Prefabs/RemotePlayer");
            var remotePlayer = Object.Instantiate(remotePlayerPrefab);

            bool isMaster = GameContainer.Common.Resolve<GameClient>().IsMaster;
            if (isMaster)
            {
                var fatMouseMovementPrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/FatMouseCharacter");
                var fatMouseMovement = Object.Instantiate(fatMouseMovementPrefab);
                fatMouseMovement.transform.SetPositionAndRotation(spawns.SpawnPoints[1].position, spawns.SpawnPoints[1].rotation);
                fatMouseMovement.Initialize(fatMouseVisuals);
                localPlayer.Initialize(fatMouseMovement, PlayerMouseType.FatMouse);
                
                remotePlayer.Initialize(thinMouseVisuals);
            }
            else
            {
                var littleMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/ThinMouseCharacter");
                var thinMouseMovement = Object.Instantiate(littleMousePrefab);
                thinMouseMovement.transform.SetPositionAndRotation(spawns.SpawnPoints[0].position, spawns.SpawnPoints[0].rotation);
                thinMouseMovement.Initialize(thinMouseVisuals);
                localPlayer.Initialize(thinMouseMovement, PlayerMouseType.ThinMouse);
                
                remotePlayer.Initialize(fatMouseVisuals);
            }
            
            GameContainer.InGame.Register<IPlayer>(localPlayer);
            GameContainer.InGame.Register(remotePlayer);
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}