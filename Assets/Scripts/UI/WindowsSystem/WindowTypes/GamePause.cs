using Common;
using GameCore.Camera;
using Startup;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class GamePause : WindowBase
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _backToMenuButton;
        [SerializeField] private Button _settingsButton;

        private void Start()
        {
            Time.timeScale = 0f;
            GameContainer.InGame.Resolve<GameCamera>().FollowTarget.SetInPause(true);
            
            _continueButton.onClick.AddListener(() =>
            {
                GameContainer.InGame.Resolve<GameCamera>().FollowTarget.SetInPause(false);
                GameContainer.Common.Resolve<WindowsSystem>().DestroyWindow<GamePause>();
            });
            
            _backToMenuButton.onClick.AddListener(() =>
            {
                GameContainer.Common.Resolve<GameInitializer>().StopGame();
                GameContainer.Common.Resolve<WindowsSystem>().DestroyWindow<GamePause>();
            });
            
            _settingsButton.onClick.AddListener(OpenSettings);
        }

        private void OpenSettings()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<SettingsScreen>();
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}