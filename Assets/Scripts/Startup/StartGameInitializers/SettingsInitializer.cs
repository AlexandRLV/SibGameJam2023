using Common.DI;
using GameCore.Camera;
using PlayerSettings;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class SettingsInitializer : InitializerBase
    {
        [SerializeField] private CameraSettings _cameraSettings;
        
        public override void Initialize()
        {
            GameContainer.Common.Register(_cameraSettings);
            
            var settingsManager = GameContainer.Create<GameSettingsManager>();
            settingsManager.Initialize();
            settingsManager.Apply();
            
            GameContainer.Common.Register(settingsManager);
        }

        public override void Dispose()
        {
        }
    }
}