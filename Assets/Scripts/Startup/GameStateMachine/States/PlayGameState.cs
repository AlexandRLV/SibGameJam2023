﻿using Common.DI;
using GameCore.LevelObjects;
using UI;
using UnityEngine.SceneManagement;

namespace Startup.GameStateMachine.States
{
    public class PlayGameState : IGameState
    {
        [Inject] private LoadingScreen _loadingScreen;
        [Inject] private GameInfo _gameInfo;
        
        public void OnEnter()
        {
            _loadingScreen.Active = true;

            GameContainer.InGame = new Container();
            var service = GameContainer.Create<LevelObjectService>();
            GameContainer.InGame.Register(service);

            SceneManager.LoadScene(_gameInfo.currentLevel.sceneName);
            _loadingScreen.Active = false;
        }

        public void OnExit()
        {
            GameContainer.InGame.Dispose();
            GameContainer.InGame = null;
        }
    }
}