using Common;
using UI.NotificationsSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class GamePause : WindowBase
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _backToMenuButton;

        private void Start()
        {
            _continueButton.onClick.AddListener(() =>
            {
                GameContainer.Common.Resolve<WindowsSystem>().DestroyWindow<GamePause>();
            });
            _backToMenuButton.onClick.AddListener(() =>
            {
                // TODO: load menu
                GameContainer.Common.Resolve<WindowsSystem>().DestroyWindow<GamePause>();
                GameContainer.Common.Resolve<NotificationsManager>().ShowNotification("THIS FEATURE NOT READY YET!", NotificationsManager.NotificationType.Top);
            });
        }
    }
}