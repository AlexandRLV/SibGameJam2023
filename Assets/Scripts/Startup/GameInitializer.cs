using System.Collections.Generic;
using Common.DI;
using Networking;
using Startup.GameStateMachine;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;

namespace Startup
{
    [DefaultExecutionOrder(-1000)]
    public class GameInitializer : MonoBehaviour
    {
        public bool InGame { get; private set; }
        public bool IsTutorial { get; private set; }
        
        [SerializeField] private List<InitializerBase> _startupInitializers = new();
        
        // private static List<InitializerBase> _singlePlayerInitializers = new()
        // {
        //     // new NetworkObjectsInitializer(), // Todo: fix it and use it
        //     new InputInitializer(),
        //     new SinglePlayerCharacterInitializer(),
        //     new RoundInitializer(),
        //     new InGameUIInitializer(),
        // };
        //
        // private static List<InitializerBase> _multiplayerInitializers = new()
        // {
        //     // new NetworkObjectsInitializer(),
        //     new InputInitializer(),
        //     new MultiplayerCharacterInitializer(),
        //     new RoundInitializer(),
        //     new InGameUIInitializer(),
        // };
        
        [Inject] private GameClientData _gameClientData;
        [Inject] private WindowsSystem _windowsSystem;

        private bool _isActive;
        private bool _wasMultiplayer;

        private GameStateMachine.GameStateMachine _gameStateMachine;

        private void Awake()
        {
            _isActive = true;
            DontDestroyOnLoad(gameObject);

            GameContainer.Common = new Container();
            GameContainer.Common.Register(this);

            foreach (var initializer in _startupInitializers)
            {
                initializer.Initialize();
            }
            
            GameContainer.InjectToInstance(this);

            _gameStateMachine = new GameStateMachine.GameStateMachine();
            _gameStateMachine.SwitchToState(GameStateType.Menu, true);
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
            if (isTutorial) StartTutorial();
            else StartPlayGame();
        }

        public void StopGame(bool toMainMenu = true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            _windowsSystem.DestroyAll();

            if (toMainMenu)
                _windowsSystem.CreateWindow<MainMenu>();
            
            InGame = false;
        }

        public void RestartGame()
        {
            StopGame(false);
            StartGame(IsTutorial);
        }

        private void StartTutorial()
        {
            InGame = true;
            _gameStateMachine.SwitchToState(GameStateType.Tutorial);
        }

        private void StartPlayGame()
        {
            InGame = true;
            bool isMultiplayer = _gameClientData.IsConnected;
            _gameStateMachine.SwitchToState(isMultiplayer ? GameStateType.MultiplayerGame : GameStateType.Game);
        }

        private void DisposeList(List<InitializerBase> initializers)
        {
            foreach (var initializer in initializers)
            {
                initializer.Dispose();
            }
        }
    }
}