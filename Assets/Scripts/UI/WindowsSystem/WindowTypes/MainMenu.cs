using Common.DI;
using Cysharp.Threading.Tasks;
using GameCore.Levels;
using PlayerProgress;
using Startup;
using UI.WindowsSystem.WindowTypes.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class MainMenu : WindowBase
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _continueGameButton;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _titlesButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _onlineButton;

        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private LevelsData _levelsData;
        [Inject] private ProgressManager _progressManager;
        [Inject] private GameInfo _gameInfo;
        [Inject] private GameInitializer _gameInitializer;
        
        private void Start()
        {
            _continueGameButton.gameObject.SetActive(_progressManager.Data.completedLevel >= 0);
            
            _startGameButton.onClick.AddListener(StartGame);
            _continueGameButton.onClick.AddListener(ContinueGame);
            _leaveButton.onClick.AddListener(LeaveGame);
            _settingsButton.onClick.AddListener(OpenSettings);
            _titlesButton.onClick.AddListener(OpenTitles);
            _onlineButton.onClick.AddListener(GoOnline);
        }
        
        private void OpenSettings()
        {
            _windowsSystem.CreateWindow<SettingsScreen>();
        }

        private void StartGame()
        {
            _progressManager.Data.completedLevel = -1;
            _progressManager.Save();
            
            _gameInfo.currentLevel = _levelsData.levels[0];
            
            if (_gameInfo.currentLevel.hasIntro)
                _windowsSystem.CreateWindow<IntroScreen>();
            else
                _gameInitializer.StartGame().Forget();
            
            _windowsSystem.DestroyWindow(this);
        }
        
        private void ContinueGame()
        {
            int nextLevelId = _progressManager.Data.completedLevel + 1;
            nextLevelId = Mathf.Min(nextLevelId, _levelsData.levels.Length - 1);
            _gameInfo.currentLevel = _levelsData.levels[nextLevelId];
            
            if (_gameInfo.currentLevel.hasIntro)
                _windowsSystem.CreateWindow<IntroScreen>();
            else
                _gameInitializer.StartGame().Forget();
            
            _windowsSystem.DestroyWindow(this);
        }

        private void OpenTitles()
        {
            _windowsSystem.CreateWindow<TitlesScreen>();
        }

        private void GoOnline()
        {
            _windowsSystem.CreateWindow<ConnectWindow>();
            _windowsSystem.DestroyWindow(this);
        }

        private void LeaveGame()
        {
            Application.Quit();
        }
    }
}