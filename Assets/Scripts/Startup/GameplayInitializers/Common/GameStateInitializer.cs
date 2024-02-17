using Common.DI;
using GameCore.LevelAchievements;
using UnityEngine;

namespace Startup.GameplayInitializers.Common
{
    public class GameStateInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var status = new LevelStatus
            {
                timeStarted = Time.time
            };
            
            GameContainer.InGame.Register(status);
        }

        public override void Dispose()
        {
        }
    }
}