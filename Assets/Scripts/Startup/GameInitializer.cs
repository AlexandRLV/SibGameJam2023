using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Startup.GameplayInitializers;
using Startup.Initializers;
using UnityEngine;

namespace Startup
{
    [DefaultExecutionOrder(-1000)]
    public class GameInitializer : MonoBehaviour
    {
        private static List<IInitializer> _startGameInitializers = new()
        {
            new GameUIInitializer(),
        };

        private static List<IInitializer> _gameplayInitializers = new()
        {
            new GameMapInitializer(),
        };
        
        public bool InGame { get; private set; }
        
        private async void Awake()
        {
            GameContainer.Common = new Container();
            GameContainer.Common.Register(this);
            
            foreach (var initializer in _startGameInitializers)
            {
                await initializer.Initialize();
            }
        }

        private void OnDestroy()
        {
            if (InGame) StopGame();
            
            foreach (var initializer in _startGameInitializers)
            {
                initializer.Dispose();
            }
        }

        public async UniTask StartGame()
        {
            GameContainer.InGame = new Container();
            foreach (var initializer in _gameplayInitializers)
            {
                await initializer.Initialize();
            }

            InGame = true;
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
    }
}