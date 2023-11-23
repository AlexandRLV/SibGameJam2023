using Common;
using GameCore.Character.Animation;
using GameCore.Player;
using GameCore.Sounds;
using LocalMessages;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class SignalizationLever : InteractiveObject
    {
        public override AnimationType InteractAnimation => AnimationType.LeverPull;
        public override InteractiveObjectType Type => InteractiveObjectType.Lever;
        
        [SerializeField] private LaserGroup laserGroup;

        public override void Interact()
        {
            if (IsUsed) return;
            
            IsUsed = true;
            DisableLasers();
            OnPlayerExit();
        }

        public override void InteractWithoutPlayer()
        {
            IsUsed = true;
            DisableLasers();
            OnPlayerExit();
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

        private void DisableLasers()
        {
            SoundService.PlaySound(SoundType.Panel);
            var message = new LaserDestroyMessage();
            message.LaserGroup = laserGroup;
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        }
    }
}