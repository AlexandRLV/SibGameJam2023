﻿using Common.DI;
using GameCore.LevelObjects;
using UI;
using UnityEngine.SceneManagement;

namespace Startup.GameStateMachine.States
{
    public class TutorialGameState : IGameState
    {
        private const string SceneName = "Level_0_tutorial";
        
        [Inject] private LoadingScreen _loadingScreen;
        
        public void OnEnter()
        {
            _loadingScreen.Active = true;

            GameContainer.InGame = new Container();
            var service = GameContainer.Create<LevelObjectService>();
            GameContainer.InGame.Register(service);

            SceneManager.LoadScene(SceneName);
            _loadingScreen.Active = false;
        }

        public void OnExit()
        {
            GameContainer.InGame.Dispose();
            GameContainer.InGame = null;
        }
    }
}