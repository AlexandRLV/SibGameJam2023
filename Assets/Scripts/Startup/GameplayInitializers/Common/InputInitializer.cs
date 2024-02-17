using Common.DI;
using GameCore.Camera;
using GameCore.Input;
using UnityEngine;

namespace Startup.GameplayInitializers.Common
{
    public class InputInitializer : InitializerBase
    {
        [SerializeField] private DesktopInputSource _desktopInputSource;
        [SerializeField] private GameCamera _gameCamera;
        
        public override void Initialize()
        {
            var inputState = new InputState();
            GameContainer.InGame.Register(inputState);

            GameContainer.InstantiateAndResolve(_desktopInputSource);
            
            var gameCamera = Instantiate(_gameCamera);
            GameContainer.InGame.Register(gameCamera);
        }

        public override void Dispose()
        {
        }
    }
}