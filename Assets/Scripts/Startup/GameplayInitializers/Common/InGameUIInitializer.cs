using Common.DI;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;

namespace Startup.GameplayInitializers.Common
{
    public class InGameUIInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateNamedWindow<InGameUI>("InGameUI");
        }

        public override void Dispose()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyWindow<InGameUI>();
        }
    }
}