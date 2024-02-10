using Common.DI;
using Localization;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class LocalizationInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var data = Resources.Load<LocalizationData>("Localization/LocalizationData");

            var provider = GameContainer.Create<LocalizationProvider>();
            provider.ReadData(data);

            provider.SetLanguage(
                provider.HasLanguage(Application.systemLanguage)
                    ? Application.systemLanguage
                    : SystemLanguage.Russian);

            GameContainer.Common.Register(provider);
        }

        public override void Dispose()
        {
        }
    }
}