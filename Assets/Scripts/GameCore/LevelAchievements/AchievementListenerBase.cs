using Common.DI;
using LocalMessages;

namespace GameCore.LevelAchievements
{
    public abstract class AchievementListenerBase
    {
        [Inject] protected LocalMessageBroker messageBroker;
        [Inject] protected LevelStatus levelStatus;
        
        public abstract void Initialize();
        public abstract void Dispose();
    }
}