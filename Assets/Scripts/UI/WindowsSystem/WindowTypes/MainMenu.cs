using Common;
using GameCore.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class MainMenu : WindowBase
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _settingsButton;
        
        private void Awake()
        {
            var soundService = GameContainer.Common.Resolve<SoundService>();
            soundService.PlayMusic(MusicType.Menu);
            _startGameButton.onClick.AddListener(StartGame);
            _leaveButton.onClick.AddListener(LeaveGame);
            _settingsButton.onClick.AddListener(OpenSettings);
        }

        private void OpenSettings()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<SettingsScreen>();
        }

        private void StartGame()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<IntroScreen>();
            windowsSystem.DestroyWindow<MainMenu>();
        }

        private void LeaveGame()
        {
            Application.Quit();
        }
    }
}