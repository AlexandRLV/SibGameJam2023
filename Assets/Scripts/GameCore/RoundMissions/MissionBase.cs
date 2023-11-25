using Common;
using Common.DI;
using GameCore.Sounds;

namespace GameCore.RoundMissions
{
    public abstract class MissionBase
    {
        public bool IsCompleted { get; private set; }
        public abstract string MissionText { get; protected set; }

        [Inject] private SoundService _soundService;
        
        protected readonly MissionsController controller;

        protected MissionBase(MissionsController controller) => this.controller = controller;

        public virtual void Update() { }
        public abstract void Dispose();

        protected void Complete()
        {
            IsCompleted = true;
            _soundService.PlaySound(SoundType.SubmissionComplete);
        }
    }
}