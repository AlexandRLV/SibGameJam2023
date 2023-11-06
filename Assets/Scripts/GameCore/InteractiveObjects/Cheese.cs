using GameCore.Character.Animation;
using GameCore.Common;
using GameCore.Sounds;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class Cheese : InteractiveObject
    {
        [SerializeField] private float speedMultiplier;
        [SerializeField] private float speedMultiplierDuration;

        public override AnimationType InteractAnimation => AnimationType.Eat;

        public override void Interact()
        {
           SoundService.PlaySound(SoundType.Eating);

            Movement.ChangeMovementSpeed(speedMultiplier, speedMultiplierDuration);
            Destroy(gameObject);
        }

        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
            if (IsSeen) return;
            IsSeen = true;
            switch (RoundController.Stage)
            {
                case RoundStage.ThinMouse:
                    SoundService.PlaySound(SoundType.ThinCheese);
                    break;
                case RoundStage.FatMouse:
                    SoundService.PlaySound(SoundType.FatCheese);
                    break;
            }
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