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

            var provider = GameContainer.Create<LocalizationProvider>();
            provider.ReadData(data);

            provider.SetLanguage(
                provider.HasLanguage(Application.systemLanguage)
                    ? Application.systemLanguage
                    : SystemLanguage.Russian);

            GameContainer.Common.Register(provider);
            
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}