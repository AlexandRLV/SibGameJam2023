using Common.DI;
using GameCore.Sounds;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class SoundServiceInitializer : InitializerBase
    {
        [SerializeField] private SoundService _soundService;
        
        public override void Initialize()
        {
            var soundService = Instantiate(_soundService);
            DontDestroyOnLoad(soundService);
            GameContainer.Common.Register(soundService);
        }

        public override void Dispose()
        {
        }
    }
}