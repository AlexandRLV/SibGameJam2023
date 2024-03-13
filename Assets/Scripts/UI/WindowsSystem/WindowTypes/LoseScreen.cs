using Common.DI;
using Cysharp.Threading.Tasks;
using GameCore.Camera;
using GameCore.Common;
using Localization;
using Networking.Client;
using Networking.Dataframes.InGame;
using Startup;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class LoseScreen : WindowBase
    {
        [SerializeField] private TextLocalizer _reasonLabelLocalizer;
        [SerializeField] private Button _restartButton;
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
                    reason = GameFinishedReason.Lose
                };
                _gameClient.Send(ref dataframe);
                return;
            }
            
            _gameCamera.FollowTarget.SetInPause(true);
            _restartButton.onClick.AddListener(() =>
            {
                _gameInitializer.RestartGame().Forget();
            });
            
            _menuButton.onClick.AddListener(() =>
            {
                _gameInitializer.StopGame().Forget();
            });
        }

        public void Initialize(LoseGameReason reason)
        {
            string key = reason switch
            {
                LoseGameReason.TimeOut => "$LOSE_SCREEN_REASON_TIMEOUT",
                LoseGameReason.Caught => "$LOSE_SCREEN_REASON_CAUGHT",
                LoseGameReason.Dead => "$LOSE_SCREEN_REASON_DIED",
                _ => "$LOSE_SCREEN_REASON_CAUGHT"
            };
            
            _reasonLabelLocalizer.SetKey(key);
        }
    }
}