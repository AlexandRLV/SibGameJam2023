using Common.DI;
using GameCore.Enemies;
using GameCore.LevelObjects.Abstract;
using GameCore.LevelObjects.InteractiveObjects;
using GameCore.LevelObjects.Messages;
using GameCore.Sounds;
using GameCore.Sounds.Playback;
using LocalMessages;
using UnityEngine;

namespace GameCore.LevelObjects.TriggerObjects
{
    public class SignalizationLaser : BaseTriggerObject
    {
        [SerializeField] private LaserGroup laserGroup;
        [SerializeField] private bool _useIntGroup;
        [SerializeField] private int _laserGroup;

        private void Start()
        {
            var messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            messageBroker.Subscribe<LaserDestroyMessage>(OnLaserDestroyed);
        }

        private void OnLaserDestroyed(ref LaserDestroyMessage value)
        {
            if (!_useIntGroup)
                _laserGroup = (int)laserGroup;
            
            if (value.LaserGroup == _laserGroup)
                Destroy(gameObject);
        }

        protected override void OnPlayerEnter()
        {
            if (IsUsed)
                return;
            
            var message = new PlayerDetectedMessage();
            message.PlayerPosition = Movement.transform.position;
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
            
            soundService.StopMusic();
            soundService.PlaySound(SoundType.Alert);
            
            IsUsed = true;
        }

        private void OnDestroy()
        {
            var messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            messageBroker.Unsubscribe<LaserDestroyMessage>(OnLaserDestroyed);
        }
    }
}