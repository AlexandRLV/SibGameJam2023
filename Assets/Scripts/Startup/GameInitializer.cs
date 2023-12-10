using System.Collections;
using System.Collections.Generic;
using Common.DI;
using Networking;
using Startup.GameplayInitializers;
using Startup.GameplayInitializers.Multiplayer;
using Startup.GameplayInitializers.Tutorial;
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

        private static List<IInitializer> _tutorialInitializers = new()
        {
            new TutorialMapInitializer(),
            new InputInitializer(),
            new TutorialCharacterInitializer(),
            new TutorialRoundInitializer(),
            new TutorialUiInitializer(),
        };

        // These guys allow you to play singleplayer
        private static List<IInitializer> _singlePlayerInitializers = new()
        {
            new GameMapInitializer(),
            // new NetworkObjectsInitializer(), // Todo: fix it and use it
            new InputInitializer(),
            new SinglePlayerCharacterInitializer(),
            new RoundInitializer(),
            new InGameUIInitializer(),
        };

        // These guys allow you to play multiplayer
        private static List<IInitializer> _multiplayerInitializers = new()
        {
            new GameMapInitializer(),
            // new NetworkObjectsInitializer(),
            new InputInitializer(),
            new MultiplayerCharacterInitializer(),
            new RoundInitializer(),
            new InGameUIInitializer(),
        };

        public bool InGame { get; private set; }
        public bool IsTutorial { get; private set; }

        [Inject] private GameClientData _gameClientData;
        [Inject] private LoadingScreen _loadingScreen;
        [Inject] private WindowsSystem _windowsSystem;

        private bool _isActive;
        private bool _wasMultiplayer;

        private void Awake()
        {
            if (_initialized)
            {
                Destroy(gameObject);
                return;
            }

            _isActive = true;
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
            if (!_isActive)
                return;
            
            if (InGame) StopGame(false);
            
            DisposeList(_startupInitializers);
            _windowsSystem.DestroyAll();
        }

        public void StartGame(bool isTutorial)
        {
            if (_gameClientData.IsConnected)
                isTutorial = false;
            
            IsTutorial = isTutorial;
            StartCoroutine(isTutorial ? StartTutorialCoroutine() : StartGameCoroutine());
        }

        public void StopGame(bool toMainMenu = true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            _windowsSystem.DestroyAll();

            if (toMainMenu)
            {
                SceneManager.LoadScene(MainMenuSceneName);
                _windowsSystem.CreateWindow<MainMenu>();
            }
            
            DisposeList(_wasMultiplayer ? _multiplayerInitializers : _singlePlayerInitializers);
            
            GameContainer.InGame = null;
            InGame = false;
        }

        public void RestartGame()
        {
            _loadingScreen.Active = true;
            
            StopGame(false);
            StartGame(IsTutorial);
        }

        private IEnumerator InitializeGameCoroutine()
        {
            yield return InitializeList(_startupInitializers);
            GameContainer.InjectToInstance(this);
        }

        private IEnumerator StartTutorialCoroutine()
        {
            _loadingScreen.Active = true;
            GameContainer.InGame = new Container();

            yield return InitializeList(_tutorialInitializers);

            _loadingScreen.Active = false;
            InGame = true;
        }

        private IEnumerator StartGameCoroutine()
        {
            _loadingScreen.Active = true;
            GameContainer.InGame = new Container();
            bool isMultiplayer = _gameClientData.IsConnected;
            
            if (isMultiplayer) yield return InitializeList(_multiplayerInitializers);
            else yield return InitializeList(_singlePlayerInitializers);

            _loadingScreen.Active = false;
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