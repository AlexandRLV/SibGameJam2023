using Common;
using GameCore.Character.Animation;
using GameCore.Sounds;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class Cheese : InteractiveObject
    {
        [SerializeField] private float speedMultiplier;
        [SerializeField] private float speedMultiplierDuration;

        private SoundService _soundService = GameContainer.Common.Resolve<SoundService>();

        public override AnimationType InteractAnimation => AnimationType.Eat;

        public override void Interact()
        {
            _soundService.PlaySound(sound);
            Movement.ChangeMovementSpeed(speedMultiplier, speedMultiplierDuration);
        }

        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
        }

        protected override void OnPlayerStay()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnPlayerExit()
        {
            Movement.MoveValues.CurrentInteractiveObject = null;
        }
    }
}