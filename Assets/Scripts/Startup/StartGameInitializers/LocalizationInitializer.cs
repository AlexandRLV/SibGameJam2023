using System.Collections;
using Common.DI;
using Localization;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class LocalizationInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var data = Resources.Load<LocalizationData>("Localization/LocalizationData");
            var config = Resources.Load<LocalizationConfig>("Localization/LocalizationConfig");

            var provider = GameContainer.Create<LocalizationProvider>();
            provider.ReadData(data);
            provider.SetLanguage(config.defaultLanguage);
            
            GameContainer.Common.Register(provider);
            
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}