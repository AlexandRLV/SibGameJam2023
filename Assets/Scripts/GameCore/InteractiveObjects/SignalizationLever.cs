using Common;
using GameCore.Character.Animation;
using GameCore.Common;
using GameCore.Sounds;
using LocalMessages;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class SignalizationLever : InteractiveObject
    {
        [SerializeField] private LaserGroup laserGroup;

        public override AnimationType InteractAnimation => AnimationType.LeverPull;

        public override void Interact()
        {
            if (IsUsed) return;
            var message = new LaserDestroyMessage();
            message.LaserGroup = laserGroup;
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
            Movement.MoveValues.CurrentInteractiveObject = null;
            IsUsed = true;
        }

        protected override void OnPlayerEnter()
        {
            if (IsUsed) return;
            Movement.MoveValues.CurrentInteractiveObject = this;
            switch (RoundController.Stage)
            {
                case RoundStage.ThinMouse:
                    SoundService.PlaySound(SoundType.ThinPanel);
                    break;
                case RoundStage.FatMouse:
                    SoundService.PlaySound(SoundType.FatPanel);
                    break;
            }
        }
    }
}