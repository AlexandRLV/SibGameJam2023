using Common;
using Common.DI;
using GameCore.Sounds;

namespace GameCore.RoundMissions
{
    public abstract class MissionBase
    {
        public bool IsCompleted { get; private set; }
        public abstract string MissionText { get; protected set; }
        
        protected MissionsController Controller;

        public MissionBase(MissionsController controller) => Controller = controller;

        public virtual void Update() { }
        public abstract void Dispose();

        protected void Complete()
        {
            IsCompleted = true;
            GameContainer.Common.Resolve<SoundService>().PlaySound(SoundType.SubmissionComplete);
        }
    }
}