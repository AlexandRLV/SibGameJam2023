using System.Collections;
using Common;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;

namespace Startup.Initializers
{
    public class MainMenuInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<MainMenu>();
            
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}