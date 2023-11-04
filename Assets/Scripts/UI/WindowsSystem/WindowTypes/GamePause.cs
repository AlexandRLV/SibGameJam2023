using Common;
using GameCore.Camera;
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
            Time.timeScale = 0f;
            GameContainer.InGame.Resolve<GameCamera>().FollowTarget.SetInPause(true);
            
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

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            GameContainer.InGame.Resolve<GameCamera>().FollowTarget.SetInPause(false);
        }
    }
}