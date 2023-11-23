using Common;
using Common.DI;
using GameCore.Sounds;
using LocalMessages;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class SignalizationLaser : BaseTriggerObject
    {
        [SerializeField] private LaserGroup laserGroup;

        private void Start()
        {
            var messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            messageBroker.Subscribe<LaserDestroyMessage>(OnLaserDestroyed);
        }

        private void OnLaserDestroyed(ref LaserDestroyMessage value)
        {
            if (value.LaserGroup == laserGroup)
                Destroy(gameObject);
        }

        protected override void OnPlayerEnter()
        {
            if (IsUsed)
                return;
            
            var message = new PlayerDetectedMessage();
            message.PlayerPosition = Movement.transform.position;
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
            
            SoundService.StopMusic();
            SoundService.PlaySound(SoundType.Alert);
            
            IsUsed = true;
        }

        private void OnDestroy()
        {
            var messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            messageBroker.Unsubscribe<LaserDestroyMessage>(OnLaserDestroyed);
        }
    }
}