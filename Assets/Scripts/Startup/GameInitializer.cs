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
        private static bool _initialized;
        
        private static List<IInitializer> _startupInitializers = new()
        {
            new MainUIInitializer(),
        };
        
        private static List<IInitializer> _mainMenuInitializers = new()
        {
            new MainMenuInitializer(),
        };

        private static List<IInitializer> _gameMapInitializers = new()
        {
            new GameMapInitializer(),
        };

        private static List<IInitializer> _gameplayInitializers = new()
        {
            new InGameUIInitializer(),
            new CharacterInitializer(),
            new InteractiveObjectsInitializer(),
        };
        
        public bool InGame { get; private set; }

        [SerializeField] private bool _initializeRightToGame;
        
        private void Awake()
        {
            if (_initialized)
            {
                Destroy(gameObject);
                return;
            }
            
            GameContainer.Common = new Container();
            GameContainer.Common.Register(this);

            StartCoroutine(InitializeGameCoroutine());
            _initialized = true;
        }

        private void OnDestroy()
        {
            if (InGame) StopGame();

            foreach (var initializer in _startupInitializers)
            {
                initializer.Dispose();
            }
            
            foreach (var initializer in _mainMenuInitializers)
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

            foreach (var initializer in _gameMapInitializers)
            {
                initializer.Dispose();
            }

            GameContainer.InGame = null;
            InGame = false;
        }

        private IEnumerator InitializeGameCoroutine()
        {
            foreach (var initializer in _startupInitializers)
            {
                yield return initializer.Initialize();
            }
            
            if (!_initializeRightToGame)
            {
                foreach (var initializer in _mainMenuInitializers)
                {
                    yield return initializer.Initialize();
                }
            }
            else
            {
                StartGame();
            }
        }

        private IEnumerator StartGameCoroutine()
        {
            var loadingScreen = GameContainer.Common.Resolve<LoadingScreen>();
            loadingScreen.Active = true;
            
            GameContainer.InGame = new Container();
            
            if (!_initializeRightToGame)
            {
                foreach (var initializer in _gameMapInitializers)
                {
                    yield return initializer.Initialize();
                }
            }
            
            foreach (var initializer in _gameplayInitializers)
            {
                yield return initializer.Initialize();
            }

            loadingScreen.Active = false;
            InGame = true;
        }
    }
}