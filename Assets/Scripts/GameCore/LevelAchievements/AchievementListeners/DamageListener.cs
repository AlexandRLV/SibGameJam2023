using GameCore.LevelAchievements.LocalMessages;

namespace GameCore.LevelAchievements.AchievementListeners
{
    public class DamageListener : AchievementListenerBase
    {
        public override void Initialize()
        {
            messageBroker.Subscribe<PlayerDamagedMessage>(OnPlayerDamaged);
        }

        public override void Dispose()
        {
            messageBroker.Unsubscribe<PlayerDamagedMessage>(OnPlayerDamaged);
        }

        private void OnPlayerDamaged(ref PlayerDamagedMessage message)
        {
            levelStatus.damaged = true;
        }
    }
}