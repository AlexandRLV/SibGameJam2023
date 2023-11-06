using Common;
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
            if (value.LaserGroup == laserGroup) Destroy(gameObject);
        }

        protected override void OnPlayerEnter()
        {
            var message = new PlayerDetectedMessage();
            message.PlayerPosition = Movement.transform.position;
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        }

        protected override void OnPlayerStay()
        {
        }

        protected override void OnPlayerExit()
        {
        }

        private void OnDestroy()
        {
            var messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            messageBroker.Unsubscribe<LaserDestroyMessage>(OnLaserDestroyed);
        }
    }
}