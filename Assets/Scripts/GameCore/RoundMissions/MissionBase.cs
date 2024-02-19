using Common.DI;
using GameCore.Sounds;

namespace GameCore.RoundMissions
{
    public abstract class MissionBase
    {
        public bool IsCompleted { get; private set; }
        public abstract string MissionLocalizationKey { get; protected set; }

        [Inject] protected SoundService soundService;
        
        protected readonly MissionsController controller;

        protected MissionBase(MissionsController controller)
        {
            this.controller = controller;
            GameContainer.InjectToInstance(this);
        }

        public virtual void Update() { }
        public abstract void Dispose();

        protected void Complete()
        {
            IsCompleted = true;
            soundService.PlaySound(SoundType.SubmissionComplete);
        }
    }
}