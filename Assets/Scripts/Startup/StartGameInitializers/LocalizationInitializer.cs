using Common.DI;
using Localization;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class LocalizationInitializer : InitializerBase
    {
        [SerializeField] private LocalizationData _localizationData;
        
        public override void Initialize()
        {
            var provider = GameContainer.Create<LocalizationProvider>();
            provider.ReadData(_localizationData);

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