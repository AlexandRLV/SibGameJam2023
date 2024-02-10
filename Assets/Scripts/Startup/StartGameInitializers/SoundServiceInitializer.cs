using Common.DI;
using GameCore.Sounds;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class SoundServiceInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var soundServicePrefab = Resources.Load<SoundService>("Audio/SoundService");
            var soundService = Instantiate(soundServicePrefab);
            DontDestroyOnLoad(soundService);
            GameContainer.Common.Register(soundService);
        }

        public override void Dispose()
        {
        }
    }
}