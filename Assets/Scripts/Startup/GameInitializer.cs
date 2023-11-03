using System.Collections;
using System.Collections.Generic;
using Common;
using Startup.GameplayInitializers;
using Startup.Initializers;
using UI;
using UnityEngine;

namespace Startup
{
    [DefaultExecutionOrder(-1000)]
    public class GameInitializer : MonoBehaviour
    {
        private static List<IInitializer> _startGameInitializers = new()
        {
            new MainUIInitializer(),
        };

        private static List<IInitializer> _gameplayInitializers = new()
        {
            new GameMapInitializer(),
            new InGameUIInitializer(),
            new CharacterInitializer(),
        };
        
        public bool InGame { get; private set; }
        
        private void Awake()
        {
            GameContainer.Common = new Container();
            GameContainer.Common.Register(this);

            StartCoroutine(InitializeGameCoroutine());
        }

        private void OnDestroy()
        {
            if (InGame) StopGame();
            
            foreach (var initializer in _startGameInitializers)
            {
                initializer.Dispose();
            }
        }

        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        public void StopGame()
        {
            foreach (var initializer in _gameplayInitializers)
            {
                initializer.Dispose();
            }

            GameContainer.InGame = null;
            InGame = false;
        }

        private IEnumerator InitializeGameCoroutine()
        {
            foreach (var initializer in _startGameInitializers)
            {
                yield return initializer.Initialize();
            }
        }

        private IEnumerator StartGameCoroutine()
        {
            var loadingScreen = GameContainer.Common.Resolve<LoadingScreen>();
            loadingScreen.Active = true;
            
            GameContainer.InGame = new Container();
            foreach (var initializer in _gameplayInitializers)
            {
                yield return initializer.Initialize();
            }

            loadingScreen.Active = false;
            InGame = true;
        }
    }
}