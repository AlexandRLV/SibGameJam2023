using GameCore.Character.Animation;

namespace GameCore.InteractiveObjects
{
    public class Mousetrap : InteractiveObject
    {
        public override AnimationType InteractAnimation => AnimationType.Hit;

        public override void Interact()
        {
            Movement.MoveValues.IsKnockdown = true;
            Movement.MoveValues.KnockdownTime = 5.0f;
        }
    }
}