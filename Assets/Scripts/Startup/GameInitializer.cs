using System.Collections;
using System.Collections.Generic;
using Common.DI;
using Networking;
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

        // These guys bring game to life
        private static List<IInitializer> _startupInitializers = new()
        {
            new ClientInitializer(),
            new SoundServiceInitializer(),
            new UIInitializer(),
        };

        // These guys allow you to play singleplayer
        private static List<IInitializer> _singlePlayerInitializers = new()
        {
            new GameMapInitializer(),
            new InputInitializer(),
            new SinglePlayerCharacterInitializer(),
            new RoundInitializer(),
            new InGameUIInitializer(),
        };

        // These guys allow you to play multiplayer
        private static List<IInitializer> _multiplayerInitializers = new()
        {
            new GameMapInitializer(),
            new InputInitializer(),
            new MultiplayerCharacterInitializer(),
            new RoundInitializer(),
            new InGameUIInitializer(),
        };

        public bool InGame { get; private set; }

        [Inject] private GameClient _gameClient;
        [Inject] private LoadingScreen _loadingScreen;
        [Inject] private WindowsSystem _windowsSystem;

        private bool _isGameController;
        private bool _wasMultiplayer;

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
            Debug.Log("Starting game!");
            StartCoroutine(StartGameCoroutine());
        }

        public void StopGame(bool toMainMenu = true)
        {
            Debug.Log($"Stopping game, to main menu: {toMainMenu}");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            _windowsSystem.DestroyAll();

            if (toMainMenu)
            {
                Debug.Log("Switching to main menu scene");
                SceneManager.LoadScene(MainMenuSceneName);
                _windowsSystem.CreateWindow<MainMenu>();
            }
            
            Debug.Log("Disposing gameplay initializers");
            DisposeList(_wasMultiplayer ? _multiplayerInitializers : _singlePlayerInitializers);
            
            GameContainer.InGame = null;
            InGame = false;
        }

        public void RestartGame()
        {
            _loadingScreen.Active = true;
            
            StopGame(false);
            StartGame();
        }

        private IEnumerator InitializeGameCoroutine()
        {
            yield return InitializeList(_startupInitializers);
            GameContainer.InjectToInstance(this);
        }

        private IEnumerator StartGameCoroutine()
        {
            Debug.Log("Starting game coroutine");
            _loadingScreen.Active = true;
            
            GameContainer.InGame = new Container();
            
            Debug.Log("Initializing gameplay");
            bool isMultiplayer = _gameClient.IsConnected;
            
            Debug.Log($"Initializing multiplayer: {isMultiplayer}");
            if (isMultiplayer) yield return InitializeList(_multiplayerInitializers);
            else yield return InitializeList(_singlePlayerInitializers);

            _loadingScreen.Active = false;
            InGame = true;
        }

        private IEnumerator InitializeList(List<IInitializer> initializers)
        {
            foreach (var initializer in initializers)
            {
                Debug.Log($"Initializing initializer {initializer.GetType().Name}");
                yield return initializer.Initialize();
            }
        }

        private void DisposeList(List<IInitializer> initializers)
        {
            foreach (var initializer in initializers)
            {
                Debug.Log($"Disposing initializer {initializer.GetType().Name}");
                initializer.Dispose();
            }
        }
    }
}