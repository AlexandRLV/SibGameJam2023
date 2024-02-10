using System.Collections;
using Common.DI;
using GameCore.Camera;
using GameCore.Input;
using UnityEngine;

namespace Startup.GameplayInitializers.Common
{
    public class InputInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var inputState = new InputState();
            GameContainer.InGame.Register(inputState);

            var inputSourcePrefab = Resources.Load<DesktopInputSource>("Input/DesktopInputSource");
            GameContainer.InstantiateAndResolve(inputSourcePrefab);
            
            var gameCameraPrefab = Resources.Load<GameCamera>("Prefabs/GameCamera");
            var gameCamera = Object.Instantiate(gameCameraPrefab);
            GameContainer.InGame.Register(gameCamera);
            
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}