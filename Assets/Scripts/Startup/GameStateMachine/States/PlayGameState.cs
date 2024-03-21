using Common.DI;
using Cysharp.Threading.Tasks;
using GameCore.LevelObjects;
using GameCore.Levels;
using UI;
using UI.NotificationsSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Startup.GameStateMachine.States
{
    public class PlayGameState : IGameState
    {
        [Inject] private LoadingScreen _loadingScreen;
        [Inject] private GameInfo _gameInfo;
        [Inject] private NotificationsManager _notificationsManager;
        [Inject] private LevelsData _levelsData;
        [Inject] private UIRoot _uiRoot;
        
        public async UniTask OnEnter()
        {
            float startTime = Time.time;
            
            _loadingScreen.Active = true;
            _loadingScreen.SetLevel(_gameInfo.currentLevel);

            GameContainer.InGame = new Container();
            var service = GameContainer.Create<LevelObjectService>();
            GameContainer.InGame.Register(service);

            await SceneManager.LoadSceneAsync(_gameInfo.currentLevel.sceneName);
            _uiRoot.UiAudioListenerState = false;
            
            float endTime = Time.time;
            float passedTime = endTime - startTime;

            float delta = _levelsData.LoadingScreenShowSeconds - passedTime;
            if (delta > 0)
                await UniTask.Delay((int)(delta * 1000));
            
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