using Common;
using Common.DI;
using GameCore.Camera;
using NetFrame.Client;
using Networking;
using Networking.Dataframes.InGame;
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
            if (!GameContainer.Common.Resolve<GameClient>().IsConnected)
                Time.timeScale = 0f;
            
            GameContainer.InGame.Resolve<GameCamera>().FollowTarget.SetInPause(true);
            
            _continueButton.onClick.AddListener(() =>
            {
                GameContainer.InGame.Resolve<GameCamera>().FollowTarget.SetInPause(false);
                GameContainer.Common.Resolve<WindowsSystem>().DestroyWindow(this);
            });
            
            _backToMenuButton.onClick.AddListener(() =>
            {
                if (GameContainer.Common.Resolve<GameClient>().IsConnected)
                {
                    var dataframe = new GameFinishedDataframe
                    {
                        reason = GameFinishedReason.Lose
                    };
                    GameContainer.Common.Resolve<NetFrameClient>().Send(ref dataframe);
                    return;
                }
                
                GameContainer.Common.Resolve<WindowsSystem>().DestroyWindow(this);
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
            if (!GameContainer.Common.Resolve<GameClient>().IsConnected)
                Time.timeScale = 1f;
        }
    }
}