using GameCore.Character.Animation;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class Mousetrap : InteractiveObject
    {
        [SerializeField] private float speedMultiplier;
        [SerializeField] private float speedMultiplierDuration;
        [SerializeField] private float distance;
        [SerializeField] private GameObject cheese;
        public override AnimationType InteractAnimation => AnimationType.Hit;

        public override void Interact()
        {
            if (IsUsed) return;
            Movement.MoveValues.IsKnockdown = true;
            Movement.MoveValues.KnockdownTime = 5.0f;
            Destroy(cheese);
            Movement.ChangeMovementSpeed(speedMultiplier, speedMultiplierDuration);
            IsUsed = true;
        }
    }
}