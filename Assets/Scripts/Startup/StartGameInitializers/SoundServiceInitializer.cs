using System.Collections;
using Common;
using GameCore.Sounds;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class SoundServiceInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var soundServicePrefab = Resources.Load<SoundService>("Audio/SoundService");
            var soundService = Object.Instantiate(soundServicePrefab);
            Object.DontDestroyOnLoad(soundService);
            GameContainer.Common.Register(soundService);
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}