﻿using System.Collections;
using Common.DI;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;

namespace Startup.GameplayInitializers.Tutorial
{
    public class TutorialUiInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateNamedWindow<InGameUI>("InGameUI_Tutorial");
            yield return null;
        }

        public void Dispose()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyWindow<InGameUI>();
        }
    }
}