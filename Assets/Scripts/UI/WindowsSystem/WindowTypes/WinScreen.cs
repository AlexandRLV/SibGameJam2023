using Common.DI;
using GameCore.Camera;
using Networking;
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
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;

        private void Start()
        {
            if (_gameClientData.IsConnected)
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
                if (_gameInitializer.IsTutorial)
                    _gameInitializer.StartGame(false);
                else
                    _gameInitializer.StopGame();
            });
        }
    }
}