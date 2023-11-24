using Common.DI;
using GameCore.Camera;
using Networking.Dataframes.InGame;
using Startup;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class WinScreen : WindowBase
    {
        [SerializeField] private Button _menuButton;

        [Inject] private GameCamera _gameCamera;
        [Inject] private GameInitializer _gameInitializer;

        private void Start()
        {
            if (_gameClient.IsConnected)
            {
                var dataframe = new GameFinishedDataframe
                {
                    reason = GameFinishedReason.Win
                };
                _gameClient.Send(ref dataframe);
                return;
            }
            
            _gameCamera.FollowTarget.SetInPause(true);
            _menuButton.onClick.AddListener(() =>
            {
                _gameInitializer.StopGame();
            });
        }
    }
}