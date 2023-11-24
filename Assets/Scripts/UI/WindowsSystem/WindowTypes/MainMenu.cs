using Common.DI;
using GameCore.Sounds;
using UI.WindowsSystem.WindowTypes.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class MainMenu : WindowBase
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _titlesButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _onlineButton;

        [Inject] private SoundService _soundService;
        [Inject] private WindowsSystem _windowsSystem;
        
        private void Start()
        {
            _soundService.PlayMusic(MusicType.Menu);

            _startGameButton.onClick.AddListener(StartGame);
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
            _windowsSystem.CreateWindow<IntroScreen>();
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