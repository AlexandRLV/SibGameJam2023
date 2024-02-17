using Common;
using Common.DI;
using GameCore.Camera;
using GameCore.LevelAchievements;
using Networking;
using Networking.Dataframes.InGame;
using Startup;
using TMPro;
using UI.WindowsSystem.WindowTypes.Extra;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class WinScreen : WindowBase
    {
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private MedalView _medalView;

        [Inject] private GameCamera _gameCamera;
        [Inject] private GameInitializer _gameInitializer;
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        [Inject] private LevelStatus _levelStatus;

        private void Start()
        {
            float time = Time.time - _levelStatus.timeStarted;
            UiUtils.SetTimerText(_timerText, time);
            _medalView.SetMedal(MedalType.Gold);
            
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
                _gameInitializer.StopGame();
            });
            _restartButton.onClick.AddListener(() =>
            {
                _gameInitializer.RestartGame();
            });
            _continueButton.onClick.AddListener(() =>
            {
                _gameInitializer.StartGame(false);
            });
        }
    }
}