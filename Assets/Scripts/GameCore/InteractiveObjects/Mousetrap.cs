using GameCore.Character.Animation;
using GameCore.Sounds;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class Mousetrap : InteractiveObject
    {
        [SerializeField] private float speedMultiplier;
        [SerializeField] private float speedMultiplierDuration;
        [SerializeField] private GameObject cheese;
        public override AnimationType InteractAnimation => AnimationType.Hit;

        public override void Interact()
        {
            if (IsUsed) return;
            SoundService.PlayRandomSound(SoundType.Mousetrap1, SoundType.Mousetrap2, SoundType.Mousetrap3);
            Destroy(cheese);
            Movement.ChangeMovementSpeed(speedMultiplier, speedMultiplierDuration);
            IsUsed = true;
        }
    }
}