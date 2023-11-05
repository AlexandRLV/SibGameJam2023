using Common;
using GameCore.Character.Animation;
using LocalMessages;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class SignalizationLever : InteractiveObject
    {
        [SerializeField] private LaserFloor laserFloor;

        public override AnimationType InteractAnimation => AnimationType.LeverPull;

        public override void Interact()
        {
            var message = new LaserDestroyMessage();
            message.LaserFloor = laserFloor;
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        }
        
    }
}