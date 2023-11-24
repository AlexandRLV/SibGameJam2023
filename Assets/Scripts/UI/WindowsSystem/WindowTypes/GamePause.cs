using Common.DI;
using GameCore.Camera;
using Networking;
using Networking.Dataframes.InGame;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class GamePause : WindowBase
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _backToMenuButton;
        [SerializeField] private Button _settingsButton;

        [Inject] private GameCamera _gameCamera;

        private void Start()
        {
            if (!_gameClient.IsConnected)
                Time.timeScale = 0f;
            
            _gameCamera.FollowTarget.SetInPause(true);
            
            _continueButton.onClick.AddListener(() =>
            {
                _gameCamera.FollowTarget.SetInPause(false);
                _windowsSystem.DestroyWindow(this);
            });
            
            _backToMenuButton.onClick.AddListener(() =>
            {
                if (_gameClient.IsConnected)
                {
                    var dataframe = new GameFinishedDataframe
                    {
                        reason = GameFinishedReason.Lose
                    };
                    _gameClient.Send(ref dataframe);
                    return;
                }
                
                _windowsSystem.DestroyWindow(this);
            });
            
            _settingsButton.onClick.AddListener(OpenSettings);
        }

        private void OpenSettings()
        {
            _windowsSystem.CreateWindow<SettingsScreen>();
        }

        private void OnDestroy()
        {
            if (!_gameClient.IsConnected)
                Time.timeScale = 1f;
        }
    }
}