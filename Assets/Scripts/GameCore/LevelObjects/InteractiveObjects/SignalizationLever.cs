using Common.DI;
using GameCore.Character.Animation;
using GameCore.LevelObjects.Abstract;
using GameCore.LevelObjects.Messages;
using GameCore.Player;
using GameCore.Sounds;
using LocalMessages;
using Startup;
using UnityEngine;

namespace GameCore.LevelObjects.InteractiveObjects
{
    public class SignalizationLever : InteractiveObject
    {
        public override AnimationType InteractAnimation => AnimationType.LeverPull;
        public override InteractiveObjectType Type => InteractiveObjectType.Lever;
        public override Vector3 CheckPosition => transform.position;
        
        [SerializeField] private LaserGroup laserGroup;
        [SerializeField] private bool _useIntGroup;
        [SerializeField] private int _laserGroup;
        
        [Inject] private LocalMessageBroker _messageBroker;
        [Inject] private GameInfo _gameInfo;

        protected override void OnInitialize()
        {
            GameContainer.InjectToInstance(this);
        }

        protected override void OnInteractInternal()
        {
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
            base.OnPlayerEnter();
            if (IsSeen)
                return;
            
            IsSeen = true;
            
            if (_gameInfo.currentLevel.id == 0) return;

            var player = GameContainer.InGame.Resolve<IPlayer>();
            soundService.PlaySound(player.MouseType == PlayerMouseType.ThinMouse ? SoundType.ThinPanel : SoundType.FatPanel);
        }

        private void DisableLasers()
        {
            soundService.PlaySound(SoundType.Panel);

            if (!_useIntGroup)
                _laserGroup = (int)laserGroup;
            
            var message = new LaserDestroyMessage
            {
                LaserGroup = _laserGroup
            };
            _messageBroker.Trigger(ref message);
        }
    }
}