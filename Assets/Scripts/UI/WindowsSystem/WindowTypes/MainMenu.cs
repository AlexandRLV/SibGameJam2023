using Common;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class MainMenu : WindowBase
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _leaveButton;
        
        private void Awake()
        {
            _startGameButton.onClick.AddListener(StartGame);
            _leaveButton.onClick.AddListener(LeaveGame);
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