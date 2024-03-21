using Common.DI;
using Cysharp.Threading.Tasks;
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
        [Inject] private UIRoot _uiRoot;
        
        public async UniTask OnEnter()
        {
            _loadingScreen.Active = true;
            
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != MainMenuScene)
                await SceneManager.LoadSceneAsync(MainMenuScene);
            
            _uiRoot.UiAudioListenerState = true;
            
            _windowsSystem.CreateWindow<MainMenu>();
            _soundService.PlayMusic(MusicType.Menu);
            _loadingScreen.Active = false;
        }

        public UniTask OnExit()
        {
            return UniTask.CompletedTask;
        }
    }
}