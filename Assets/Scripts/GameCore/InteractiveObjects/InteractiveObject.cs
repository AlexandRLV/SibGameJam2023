using GameCore.Character.Animation;

namespace GameCore.InteractiveObjects
{
    public abstract class InteractiveObject : BaseTriggerObject
    {
        public abstract AnimationType InteractAnimation { get; }
        public abstract void Interact();

        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
        }

        protected override void OnPlayerStay()
        {
        }

        protected override void OnPlayerExit()
        {
            Movement.MoveValues.CurrentInteractiveObject = null;
        }
    }
}