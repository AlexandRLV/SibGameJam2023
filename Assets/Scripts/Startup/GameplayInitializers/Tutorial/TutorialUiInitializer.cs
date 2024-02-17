using Common.DI;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;

namespace Startup.GameplayInitializers.Tutorial
{
    public class TutorialUiInitializer : InitializerBase
    {
        private const string WindowName = "InGameUI_Tutorial";
        
        public override void Initialize()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateNamedWindow<InGameUI>(WindowName);
        }

        public override void Dispose()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyWindow<InGameUI>();
        }
    }
}