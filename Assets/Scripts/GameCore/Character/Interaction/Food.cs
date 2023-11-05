using GameCore.Character.Animation;

namespace GameCore.Character.Interaction
{
    public class Food : InteractableObject
    {
        public override AnimationType InteractAnimation => AnimationType.Eat;

        public override void Interact()
        {
            // TODO: implement change movement speed
            // enteredMovement.ChangeMovementSpeed();
        }
    }
}