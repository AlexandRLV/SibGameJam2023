using Common;
using Startup;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class MainMenu : WindowBase
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _leaveButton;

        private GameInitializer _gameInitializer;
        
        private void Awake()
        {
            _gameInitializer = GameContainer.Common.Resolve<GameInitializer>();
            
            _startGameButton.onClick.AddListener(StartGame);
            _leaveButton.onClick.AddListener(LeaveGame);
        }

        private void StartGame()
        {
            _gameInitializer.StartGame();

            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyWindow<MainMenu>();
        }

        private void LeaveGame()
        {
            Application.Quit();
        }
    }
}