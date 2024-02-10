using Common.DI;
using GameCore.LevelObjects;
using UI;
using UnityEngine.SceneManagement;

namespace Startup.GameStateMachine.States
{
    public class PlayGameState : IGameState
    {
        private const string SceneName = "Level_01";
        
        [Inject] private LoadingScreen _loadingScreen;
        
        public void OnEnter()
        {
            _loadingScreen.Active = true;

            GameContainer.InGame = new Container();
            var service = GameContainer.Create<LevelObjectService>();
            GameContainer.InGame.Register(service);

            SceneManager.LoadScene(SceneName);
        }

        public void OnExit()
        {
            GameContainer.InGame.Dispose();
            GameContainer.InGame = null;
        }
    }
}