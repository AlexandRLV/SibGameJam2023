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
            
            var littleMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/Little Mouse Character");
            var littleMouse = Object.Instantiate(littleMousePrefab);
            littleMouse.transform.SetPositionAndRotation(spawns.SpawnPoints[0].position, spawns.SpawnPoints[0].rotation);
            
            var bigMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/Big Mouse Character");
            var bigMouse = Object.Instantiate(bigMousePrefab);
            bigMouse.transform.SetPositionAndRotation(spawns.SpawnPoints[1].position, spawns.SpawnPoints[1].rotation);

            var gameCameraPrefab = Resources.Load<GameCamera>("Prefabs/GameCamera");
            var gameCamera = Object.Instantiate(gameCameraPrefab);
            GameContainer.InGame.Register(gameCamera);
            
            var playerPrefab = Resources.Load<Player>("Prefabs/Player");
            var player = Object.Instantiate(playerPrefab);
            player.RegisterPosessableMovement(littleMouse);
            player.RegisterPosessableMovement(bigMouse);
            player.Initialize();
            
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}