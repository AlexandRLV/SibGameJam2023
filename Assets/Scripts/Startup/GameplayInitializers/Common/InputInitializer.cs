using Common.DI;
using GameCore.Camera;
using GameCore.Input;
using UnityEngine;

namespace Startup.GameplayInitializers.Common
{
    public class InputInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var inputState = new InputState();
            GameContainer.InGame.Register(inputState);

            var inputSourcePrefab = Resources.Load<DesktopInputSource>("Input/DesktopInputSource");
            GameContainer.InstantiateAndResolve(inputSourcePrefab);
            
            var gameCameraPrefab = Resources.Load<GameCamera>("Prefabs/GameCamera");
            var gameCamera = Instantiate(gameCameraPrefab);
            GameContainer.InGame.Register(gameCamera);
        }

        public override void Dispose()
        {
        }
    }
}