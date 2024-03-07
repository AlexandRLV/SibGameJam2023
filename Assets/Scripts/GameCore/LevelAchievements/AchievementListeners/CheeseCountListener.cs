using GameCore.LevelAchievements.LocalMessages;

namespace GameCore.LevelAchievements.AchievementListeners
{
    public class CheeseCountListener : AchievementListenerBase
    {
        public override void Initialize()
        {
            messageBroker.Subscribe<CheeseCollectedMessage>(OnCheeseCollected);
        }

        public override void Dispose()
        {
            messageBroker.Unsubscribe<CheeseCollectedMessage>(OnCheeseCollected);
        }

        private void OnCheeseCollected(ref CheeseCollectedMessage message)
        {
            levelStatus.cheeseCount++;
        }
    }
}