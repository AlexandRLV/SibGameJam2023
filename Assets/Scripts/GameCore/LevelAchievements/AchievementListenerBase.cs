using Common.DI;
using LocalMessages;

namespace GameCore.LevelAchievements
{
    public abstract class AchievementListenerBase
    {
        [Inject] protected LocalMessageBroker messageBroker;

        public void Initialize()
        {
            GameContainer.InjectToInstance(this);
            OnInitializeInternal();
        }
        
        protected abstract void OnInitializeInternal();
        public abstract void Dispose();
    }
}