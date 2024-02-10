using Common.DI;
using GameCore.Sounds;
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
        
        public void OnEnter()
        {
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != MainMenuScene)
                SceneManager.LoadScene(MainMenuScene);
            
            _windowsSystem.CreateWindow<MainMenu>();
            _soundService.PlayMusic(MusicType.Menu);
        }

        public void OnExit()
        {
        }
    }
}