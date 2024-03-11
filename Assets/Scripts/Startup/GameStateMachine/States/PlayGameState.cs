using Common.DI;
using Cysharp.Threading.Tasks;
using GameCore.LevelObjects;
using UI;
using UI.NotificationsSystem;
using UnityEngine.SceneManagement;

namespace Startup.GameStateMachine.States
{
    public class PlayGameState : IGameState
    {
        [Inject] private LoadingScreen _loadingScreen;
        [Inject] private GameInfo _gameInfo;
        [Inject] private NotificationsManager _notificationsManager;
        
        public async UniTask OnEnter()
        {
            _loadingScreen.Active = true;
            _loadingScreen.SetLevel(_gameInfo.currentLevel);

            GameContainer.InGame = new Container();
            var service = GameContainer.Create<LevelObjectService>();
            GameContainer.InGame.Register(service);

            await SceneManager.LoadSceneAsync(_gameInfo.currentLevel.sceneName);
            _loadingScreen.Active = false;
        }

        public UniTask OnExit()
        {
            _notificationsManager.ClearAll();
            GameContainer.InGame.Dispose();
            GameContainer.InGame = null;
            return UniTask.CompletedTask;
        }
    }
}