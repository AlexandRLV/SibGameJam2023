using Common.DI;
using GameCore.LevelObjects.FloorTypeDetection;
using GameCore.Sounds;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class SoundServiceInitializer : InitializerBase
    {
        [SerializeField] private SoundService _soundService;
        [SerializeField] private FloorTypesConfig _floorTypesConfig;
        
        public override void Initialize()
        {
            var soundService = Instantiate(_soundService);
            DontDestroyOnLoad(soundService);
            GameContainer.Common.Register(soundService);
            
            _floorTypesConfig.Initialize();
            GameContainer.Common.Register(_floorTypesConfig);
        }

        public override void Dispose()
        {
        }
    }
}