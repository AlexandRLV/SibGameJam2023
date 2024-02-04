using System.Collections;
using Common;
using Common.DI;
using UI;
using UI.NotificationsSystem;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;

namespace Startup.Initializers
{
    public class UIInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var uiRootPrefab = Resources.Load<UIRoot>("UI/UIRoot");
            var uiRoot = Object.Instantiate(uiRootPrefab);
            Object.DontDestroyOnLoad(uiRoot);
            GameContainer.Common.Register(uiRoot);
            
            var gameWindows = Resources.Load<GameWindows>("Game Windows");
            GameContainer.Common.Register(gameWindows);
            
            var windowsSystem = GameContainer.Create<WindowsSystem>();
            GameContainer.Common.Register(windowsSystem);
            
            var loadingScreenPrefab = Resources.Load<LoadingScreen>("UI/LoadingScreen");
            var loadingScreen = Object.Instantiate(loadingScreenPrefab, uiRoot.OverlayParent);
            loadingScreen.Active = true;
            GameContainer.Common.Register(loadingScreen);

            var notificationsManagerPrefab = Resources.Load<NotificationsManager>("UI/NotificationsScreen");
            var notificationsManager = Object.Instantiate(notificationsManagerPrefab, uiRoot.NotificationsParent);
            GameContainer.Common.Register(notificationsManager);
            
            windowsSystem.CreateWindow<MainMenu>();

            yield return null;
        }

        public void Dispose()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyAll();
        }
    }
}