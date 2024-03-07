using Common;
using Common.DI;
using GameCore.Camera;
using GameCore.LevelAchievements;
using GameCore.LevelAchievements.AchievementView;
using GameCore.Levels;
using Networking.Client;
using Networking.Dataframes.InGame;
using PlayerProgress;
using Startup;
using TMPro;
using UI.NotificationsSystem;
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
        [SerializeField] private CheeseAchievementView _cheeseAchievementView;
        [SerializeField] private HpAchievementView _hpAchievementView;

        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private GameCamera _gameCamera;
        [Inject] private GameInitializer _gameInitializer;
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        [Inject] private LevelStatus _levelStatus;
        [Inject] private ProgressManager _progressManager;
        [Inject] private GameInfo _gameInfo;
        [Inject] private LevelsData _levelsData;
        [Inject] private NotificationsManager _notificationsManager;

        private void Start()
        {
            _notificationsManager.ClearQueue();
            
            _cheeseAchievementView.Setup();
            _hpAchievementView.Setup();
            
            float time = Time.time - _levelStatus.timeStarted;
            UiUtils.SetTimerText(_timerText, time);

            var medalType = MedalType.Bronze;
            if (_levelStatus.cheeseCount >= _gameInfo.currentLevel.cheeseCount)
            {
                medalType = MedalType.Silver;
                if (!_levelStatus.damaged)
                    medalType = MedalType.Gold;
            }
            
            _medalView.SetMedal(medalType);
            
            if (_gameClientData.IsConnected)
            {
                var dataframe = new GameFinishedDataframe
                {
                    reason = GameFinishedReason.Win
                };
                _gameClient.Send(ref dataframe);
                return;
            }

            _progressManager.Data.completedLevel =
                Mathf.Max(_progressManager.Data.completedLevel, _gameInfo.currentLevel.id);
            _progressManager.Save();

            bool isLastLevel = _gameInfo.currentLevel.id >= _levelsData.levels.Length - 1;
            if (isLastLevel)
                _continueButton.gameObject.SetActive(false);
            
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
                if (_gameInfo.currentLevel.id < _levelsData.levels.Length - 1)
                {
                    int nextLevelId = _gameInfo.currentLevel.id + 1;
                    _gameInfo.currentLevel = _levelsData.levels[nextLevelId];
                }
                _windowsSystem.DestroyWindow(this);
                
                if (_gameInfo.currentLevel.hasIntro)
                    _windowsSystem.CreateWindow<IntroScreen>();
                else
                    _gameInitializer.StartGame();
            });
        }
    }
}