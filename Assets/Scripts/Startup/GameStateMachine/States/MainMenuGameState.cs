using Common.DI;
using GameCore.Sounds;
using UI;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine.SceneManagement;

namespace Startup.GameStateMachine.States
{
    public class MainMenuGameState : IGameState
    {
        private const string MainMenuScene = "MainMenuScene";
        
        [Inject] private SoundService _soundService;
        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private LoadingScreen _loadingScreen;
        
        public void OnEnter()
        {
            _loadingScreen.Active = true;
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != MainMenuScene)
                SceneManager.LoadScene(MainMenuScene);
            
            _windowsSystem.CreateWindow<MainMenu>();
            _soundService.PlayMusic(MusicType.Menu);
            _loadingScreen.Active = false;
        }

        public void OnExit()
        {
        }
    }
}