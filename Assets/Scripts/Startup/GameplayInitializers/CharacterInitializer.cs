using System.Collections;
using Common;
using GameCore;
using GameCore.Camera;
using GameCore.Character.Movement;
using GameCore.Input;
using GameCore.Player;
using UnityEngine;

namespace Startup.GameplayInitializers
{
    public class CharacterInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var inputState = new InputState();
            GameContainer.InGame.Register(inputState);

            var inputSourcePrefab = Resources.Load<DesktopInputSource>("Input/DesktopInputSource");
            Object.Instantiate(inputSourcePrefab);

            var spawns = GameContainer.InGame.Resolve<PlayerSpawns>();

            var littleMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/ThinMouseCharacter");
            var thinMouse = Object.Instantiate(littleMousePrefab);
            thinMouse.transform.SetPositionAndRotation(spawns.SpawnPoints[0].position, spawns.SpawnPoints[0].rotation);

            var bigMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/FatMouseCharacter");
            var fatMouse = Object.Instantiate(bigMousePrefab);
            fatMouse.transform.SetPositionAndRotation(spawns.SpawnPoints[1].position, spawns.SpawnPoints[1].rotation);
            
            var gameCameraPrefab = Resources.Load<GameCamera>("Prefabs/GameCamera");
            var gameCamera = Object.Instantiate(gameCameraPrefab);
            GameContainer.InGame.Register(gameCamera);

            var playerPrefab = Resources.Load<GamePlayer>("Prefabs/Player");
            var player = Object.Instantiate(playerPrefab);
            player.Initialize(fatMouse, thinMouse);

            GameContainer.InGame.Register(player);

            yield return null;
        }

        public void Dispose()
        {
        }
    }
}