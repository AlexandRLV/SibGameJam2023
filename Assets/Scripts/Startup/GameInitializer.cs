using System.Collections.Generic;
using Common.DI;
using Cysharp.Threading.Tasks;
using Networking.Client;
using Startup.GameStateMachine;
using UI.WindowsSystem;
using UnityEngine;

namespace Startup
{
    [DefaultExecutionOrder(-1000)]
    public class GameInitializer : MonoBehaviour
    {
        public bool InGame { get; private set; }
        
        [SerializeField] private List<InitializerBase> _startupInitializers = new();
        
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
            _gameStateMachine.SwitchToState(GameStateType.Menu, true).Forget();
        }

        private void OnDestroy()
        {
            if (!_isActive)
                return;
            
            DisposeList(_startupInitializers);
            _windowsSystem.DestroyAll();
        }

        public async UniTask StartGame()
        {
            InGame = true;
            await _gameStateMachine.SwitchToState(GameStateType.Game, true);
        }

        public async UniTask StopGame(bool toMainMenu = true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            _windowsSystem.DestroyAll();

            if (toMainMenu)
                await _gameStateMachine.SwitchToState(GameStateType.Menu);
            
            InGame = false;
        }

        public async UniTask RestartGame()
        {
            await StopGame(false);
            await StartGame();
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