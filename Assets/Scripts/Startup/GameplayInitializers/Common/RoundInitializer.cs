using Common.DI;
using GameCore.RoundControl;
using UnityEngine;

namespace Startup.GameplayInitializers.Common
{
    public class RoundInitializer : InitializerBase
    {
        [SerializeField] private RoundSettings _roundSettings;
        
        public override void Initialize()
        {
            GameContainer.InGame.Register(_roundSettings);

            var roundController = GameContainer.CreateGameObjectWithComponent<RoundController>("RoundController");
            GameContainer.InGame.Register(roundController);
        }

        public override void Dispose()
        {
        }
    }
}