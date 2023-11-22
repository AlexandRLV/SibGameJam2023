using System.Collections;
using System.Collections.Generic;
using Common;
using NetFrame.Client;
using Networking;
using Networking.Dataframes.InGame;
using Startup.GameplayInitializers;
using Startup.GameplayInitializers.Multiplayer;
using Startup.Initializers;
using Startup.StartGameInitializers;
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
            new ClientInitializer(),
            new SoundServiceInitializer(),
            new UIInitializer(),
        };

        private static List<IInitializer> _gameplayInitializers = new()
        {
            new GameMapInitializer(),
            new InGameUIInitializer(),
            new InputInitializer(),
        };

        private static List<IInitializer> _singlePlayerInitializers = new()
        {
            new SinglePlayerCharacterInitializer(),
            new RoundInitializer(),
        };

        private static List<IInitializer> _multiplayerInitializers = new()
        {
            new MultiplayerCharacterInitializer(),
            new RoundInitializer(),
        };

        public bool InGame { get; private set; }

        private bool _isGameController;

        private void Awake()
        {
            if (_initialized)
            {
                Destroy(gameObject);
                return;
            }

            _isGameController = true;
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != MainMenuSceneName)
            {
                Debug.LogError($"Пожалуйста, зайдите в игру со сцены {MainMenuSceneName}");
                Application.Quit();
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
            if (!_isGameController)
                return;
            
            if (InGame) StopGame();

            DisposeList(_startupInitializers);
        }

        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        public void StopGame(bool toMainMenu = true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyAll();

            if (toMainMenu)
            {
                SceneManager.LoadScene(MainMenuSceneName);
                windowsSystem.CreateWindow<MainMenu>();
            }
            
            DisposeList(_gameplayInitializers);

            GameContainer.InGame = null;
            InGame = false;
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
        }

        private IEnumerator StartGameCoroutine()
        {
            var loadingScreen = GameContainer.Common.Resolve<LoadingScreen>();
            loadingScreen.Active = true;
            
            GameContainer.InGame = new Container();
            
            yield return InitializeList(_gameplayInitializers);

            bool isMultiplayer = GameContainer.Common.Resolve<GameClient>().IsConnected;
            if (isMultiplayer) yield return InitializeList(_multiplayerInitializers);
            else yield return InitializeList(_singlePlayerInitializers);

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