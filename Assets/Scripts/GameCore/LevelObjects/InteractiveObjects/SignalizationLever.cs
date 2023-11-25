using Common.DI;
using GameCore.Character.Animation;
using GameCore.LevelObjects.Abstract;
using GameCore.LevelObjects.Messages;
using GameCore.Player;
using GameCore.Sounds;
using LocalMessages;
using UnityEngine;

namespace GameCore.LevelObjects.InteractiveObjects
{
    public class SignalizationLever : InteractiveObject
    {
        public override AnimationType InteractAnimation => AnimationType.LeverPull;
        public override InteractiveObjectType Type => InteractiveObjectType.Lever;
        public override Vector3 CheckPosition => transform.position;
        
        [SerializeField] private LaserGroup laserGroup;
        
        [Inject] private LocalMessageBroker _messageBroker;

        protected override void OnInitialize()
        {
            GameContainer.InjectToInstance(this);
        }

        public override void Interact()
        {
            if (IsUsed) return;
            
            IsUsed = true;
            DisableLasers();
            OnPlayerExit();
        }

        public override void InteractWithoutPlayer(Vector3 playerPosition)
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
            soundService.PlaySound(player.MouseType == PlayerMouseType.ThinMouse ? SoundType.ThinPanel : SoundType.FatPanel);
        }

        private void DisableLasers()
        {
            soundService.PlaySound(SoundType.Panel);
            
            var message = new LaserDestroyMessage
            {
                LaserGroup = laserGroup
            };
            _messageBroker.Trigger(ref message);
        }
    }
}