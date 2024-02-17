using GameCore.LevelAchievements.LocalMessages;

namespace GameCore.LevelAchievements.AchievementListeners
{
    public class CheeseCountListener : AchievementListenerBase
    {
        private int _collectedCount;
        
        protected override void OnInitializeInternal()
        {
            messageBroker.Subscribe<CheeseCollectedMessage>(OnCheeseCollected);
        }

        public override void Dispose()
        {
            messageBroker.Unsubscribe<CheeseCollectedMessage>(OnCheeseCollected);
        }

        private void OnCheeseCollected(ref CheeseCollectedMessage message)
        {
            _collectedCount++;
        }
    }
}