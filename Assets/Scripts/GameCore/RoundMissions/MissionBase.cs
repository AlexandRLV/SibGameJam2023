namespace GameCore.RoundMissions
{
    public abstract class MissionBase
    {
        public bool IsCompleted { get; protected set; }
        public abstract string MissionText { get; protected set; }
        
        protected MissionsController Controller;

        public MissionBase(MissionsController controller) => Controller = controller;

        public virtual void Update() { }
        public abstract void Dispose();
    }
}