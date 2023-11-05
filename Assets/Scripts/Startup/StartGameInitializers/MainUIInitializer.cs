using System.Collections;
using Common;
using LocalMessages;
using UI;
using UI.NotificationsSystem;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;

namespace Startup.Initializers
{
    public class MainUIInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var _messageBrocker = new LocalMessageBroker();
            GameContainer.Common.Register(_messageBrocker);

            var uiRootPrefab = Resources.Load<UIRoot>("UI/UIRoot");
            var uiRoot = Object.Instantiate(uiRootPrefab);
            GameContainer.Common.Register(uiRoot);
            
            var gameWindows = Resources.Load<GameWindows>("WindowsSystem/Game Windows");
            var windowsSystem = new WindowsSystem();
            windowsSystem.Initialize(gameWindows, uiRoot);
            GameContainer.Common.Register(windowsSystem);
            
            var loadingScreenPrefab = Resources.Load<LoadingScreen>("UI/LoadingScreen");
            var loadingScreen = Object.Instantiate(loadingScreenPrefab, uiRoot.OverlayParent);
            loadingScreen.Active = false;
            GameContainer.Common.Register(loadingScreen);

            var notificationsManagerPrefab = Resources.Load<NotificationsManager>("UI/NotificationsScreen");
            var notificationsManager = Object.Instantiate(notificationsManagerPrefab, uiRoot.NotificationsParent);
            GameContainer.Common.Register(notificationsManager);

            yield return null;
        }

        public void Dispose()
        {
        }
    }
}