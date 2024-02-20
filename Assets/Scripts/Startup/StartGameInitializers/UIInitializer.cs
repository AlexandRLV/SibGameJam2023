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
        [SerializeField] private UIRoot _uiRoot;
        [SerializeField] private GameWindows _gameWindows;
        [SerializeField] private LoadingScreen _loadingScreen;
        [SerializeField] private NotificationsManager _notificationsManager;
        
        public override void Initialize()
        {
            GameContainer.Common.Register(_gameWindows);
            
            var uiRoot = Instantiate(_uiRoot);
            DontDestroyOnLoad(uiRoot);
            GameContainer.Common.Register(uiRoot);
            
            var windowsSystem = GameContainer.Create<WindowsSystem>();
            GameContainer.Common.Register(windowsSystem);
            
            var loadingScreen = Instantiate(_loadingScreen, uiRoot.OverlayParent);
            loadingScreen.Active = true;
            GameContainer.Common.Register(loadingScreen);

            var notificationsManager = GameContainer.InstantiateAndResolve(_notificationsManager, uiRoot.NotificationsParent);
            GameContainer.Common.Register(notificationsManager);
        }

        public override void Dispose()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyAll();
        }
    }
}