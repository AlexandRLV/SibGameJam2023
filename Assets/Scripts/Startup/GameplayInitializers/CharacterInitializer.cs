using System.Collections;
using Common;
using GameCore;
using GameCore.Camera;
using GameCore.Input;
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
            
            var characterPrefab = Resources.Load<GameObject>("Prefabs/Character");
            var character = Object.Instantiate(characterPrefab);
            character.transform.SetPositionAndRotation(spawns.SpawnPoint.position, spawns.SpawnPoint.rotation);

            var gameCameraPrefab = Resources.Load<GameCamera>("Prefabs/GameCamera");
            var gameCamera = Object.Instantiate(gameCameraPrefab);
            gameCamera.SetTarget(character.transform);
            
            GameContainer.InGame.Register(gameCamera);
            
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}