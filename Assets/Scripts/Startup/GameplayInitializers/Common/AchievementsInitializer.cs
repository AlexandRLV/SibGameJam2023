using Common.DI;
using GameCore.LevelAchievements;
using GameCore.LevelAchievements.AchievementListeners;

namespace Startup.GameplayInitializers.Common
{
    public class AchievementsInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var listener = new AchievementsListener();
            listener.AddAchievementListener(GameContainer.Create<CheeseCountListener>());
            listener.AddAchievementListener(GameContainer.Create<DamageListener>());
            
            GameContainer.InGame.Register(listener);
        }

        public override void Dispose()
        {
        }
    }
}