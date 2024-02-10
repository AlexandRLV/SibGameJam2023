using Common.DI;
using UI;
using UI.NotificationsSystem;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class UIInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var uiRootPrefab = Resources.Load<UIRoot>("UI/UIRoot");
            var uiRoot = Instantiate(uiRootPrefab);
            DontDestroyOnLoad(uiRoot);
            GameContainer.Common.Register(uiRoot);
            
            var gameWindows = Resources.Load<GameWindows>("Game Windows");
            GameContainer.Common.Register(gameWindows);
            
            var windowsSystem = GameContainer.Create<WindowsSystem>();
            GameContainer.Common.Register(windowsSystem);
            
            var loadingScreenPrefab = Resources.Load<LoadingScreen>("UI/LoadingScreen");
            var loadingScreen = Instantiate(loadingScreenPrefab, uiRoot.OverlayParent);
            loadingScreen.Active = true;
            GameContainer.Common.Register(loadingScreen);

            var notificationsManagerPrefab = Resources.Load<NotificationsManager>("UI/NotificationsScreen");
            var notificationsManager = Instantiate(notificationsManagerPrefab, uiRoot.NotificationsParent);
            GameContainer.Common.Register(notificationsManager);
        }

        public override void Dispose()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyAll();
        }
    }
}