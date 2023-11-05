using GameCore.Character.Animation;

namespace GameCore.Character.Interaction
{
    public abstract class InteractableObject : BaseTriggerObject
    {
        public abstract AnimationType InteractAnimation { get; }
        
        protected override void OnCharacterEnter()
        {
            enteredMovement.MoveValues.CurrentInteractable = this;
        }

        protected override void OnCharacterExit()
        {
            enteredMovement.MoveValues.CurrentInteractable = null;
        }

        public abstract void Interact();
    }
}