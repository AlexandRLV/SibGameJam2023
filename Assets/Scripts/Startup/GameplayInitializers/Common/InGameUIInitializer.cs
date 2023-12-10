using System.Collections;
using Common.DI;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;

namespace Startup.GameplayInitializers
{
    public class InGameUIInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateNamedWindow<InGameUI>("InGameUI");
            yield break;
        }

        public void Dispose()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyWindow<InGameUI>();
        }
    }
}