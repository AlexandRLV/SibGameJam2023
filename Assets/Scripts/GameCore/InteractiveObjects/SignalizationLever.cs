using Common;
using GameCore.Character.Animation;
using GameCore.Common;
using GameCore.Player;
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
            SoundService.PlaySound(SoundType.Panel);
            var message = new LaserDestroyMessage();
            message.LaserGroup = laserGroup;
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
            Movement.MoveValues.CurrentInteractiveObject = null;
            IsUsed = true;
        }

        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
            if (IsSeen)
                return;
            
            IsSeen = true;
            
            var player = GameContainer.InGame.Resolve<IPlayer>();
            SoundService.PlaySound(player.MouseType == PlayerMouseType.ThinMouse ? SoundType.ThinPanel : SoundType.FatPanel);
        }
    }
}