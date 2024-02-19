using Common.DI;
using GameCore.Levels;
using PlayerProgress;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class ProgressInitializer : InitializerBase
    {
        [SerializeField] private LevelsData _levelsData;
        
        public override void Initialize()
        {
            var progressManager = new ProgressManager();
            progressManager.Initialize();
            GameContainer.Common.Register(progressManager);

            var gameInfo = new GameInfo();
            GameContainer.Common.Register(gameInfo);
            
            _levelsData.Initialize();
            GameContainer.Common.Register(_levelsData);
        }

        public override void Dispose()
        {
            
        }
    }
}