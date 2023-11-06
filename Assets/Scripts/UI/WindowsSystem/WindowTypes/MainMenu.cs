using Common;
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
        
        private void Awake()
        {
            _startGameButton.onClick.AddListener(StartGame);
            _leaveButton.onClick.AddListener(LeaveGame);
            _settingsButton.onClick.AddListener(OpenSettings);
            _titlesButton.onClick.AddListener(OpenTitles);
        }

        private void OpenSettings()
        {
            GameContainer.Common.Resolve<WindowsSystem>().CreateWindow<SettingsScreen>();
        }

        private void StartGame()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<IntroScreen>();
            windowsSystem.DestroyWindow<MainMenu>();
        }

        private void OpenTitles()
        {
            GameContainer.Common.Resolve<WindowsSystem>().CreateWindow<TitlesScreen>();
        }

        private void LeaveGame()
        {
            Application.Quit();
        }
    }
}