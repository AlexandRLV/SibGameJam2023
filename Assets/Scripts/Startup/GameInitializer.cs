using System.Collections;
using System.Collections.Generic;
using Common;
using GameCore.Camera;
using Startup.GameplayInitializers;
using Startup.Initializers;
using UI;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Startup
{
    [DefaultExecutionOrder(-1000)]
    public class GameInitializer : MonoBehaviour
    {
        private const string MainMenuSceneName = "MainMenuScene";
        private static bool _initialized;

        private static List<IInitializer> _startupInitializers = new()
        {
            new MainUIInitializer(),
            new SoundServiceInitializer(),
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
            new RoundInitializer(),
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

            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != MainMenuSceneName)
            {
                Debug.LogError($"Пожалуйста, зайдите в игру со сцены {MainMenuSceneName}");
                return;
            }

            DontDestroyOnLoad(gameObject);

            GameContainer.Common = new Container();
            GameContainer.Common.Register(this);

            StartCoroutine(InitializeGameCoroutine());
            _initialized = true;
        }

        private void OnDestroy()
        {
            if (InGame) StopGame();

            DisposeList(_startupInitializers);
            DisposeList(_mainMenuInitializers);
        }

        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        public void StopGame(bool toMainMenu = true)
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyAll();
            
            DisposeList(_gameplayInitializers);
            DisposeList(_gameMapInitializers);

            GameContainer.InGame = null;
            InGame = false;
            
            if (!toMainMenu) return;
            
            windowsSystem.CreateWindow<MainMenu>();
        }

        public void RestartGame()
        {
            var loadingScreen = GameContainer.Common.Resolve<LoadingScreen>();
            loadingScreen.Active = true;
            
            StopGame(false);
            StartGame();
        }

        private IEnumerator InitializeGameCoroutine()
        {
            yield return InitializeList(_startupInitializers);

            if (!_initializeRightToGame)
                yield return InitializeList(_mainMenuInitializers);
            else
                StartGame();
        }

        private IEnumerator StartGameCoroutine()
        {
            var loadingScreen = GameContainer.Common.Resolve<LoadingScreen>();
            loadingScreen.Active = true;
            
            GameContainer.InGame = new Container();
            
            if (!_initializeRightToGame)
                yield return InitializeList(_gameMapInitializers);

            yield return InitializeList(_gameplayInitializers);

            loadingScreen.Active = false;
            InGame = true;
        }

        private IEnumerator InitializeList(List<IInitializer> initializers)
        {
            foreach (var initializer in initializers)
            {
                yield return initializer.Initialize();
            }
        }

        private void DisposeList(List<IInitializer> initializers)
        {
            foreach (var initializer in initializers)
            {
                initializer.Dispose();
            }
        }
    }
}