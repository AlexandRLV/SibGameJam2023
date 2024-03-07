using Common;
using Common.DI;
using GameCore.Common.Messages;
using GameCore.LevelObjects.Abstract;
using GameCore.LevelObjects.Messages;
using LocalMessages;
using UI.NotificationsSystem;
using UnityEngine;

namespace GameCore.LevelObjects.InteractiveObjects
{
    public class EvacuationInteractive : BaseTriggerObject
    {
        private const float TimeToShowEvacuateNotif = 3f;
        
        [Inject] private LocalMessageBroker _messageBroker;
        [Inject] private LevelObjectService _levelObjectService;
        [Inject] private NotificationsManager _notificationsManager;

        private float _timer;
        
        private void Start()
        {
            GameContainer.InjectToInstance(this);
            
            _messageBroker.Subscribe<ActivateEvacuationMessage>(OnEvacuationActivated);
            _levelObjectService.evacuation = this;
            
            gameObject.SetActive(false);
        }

        private void Update()
        {
            // if (IsUsed) return;
            // if (_timer <= 0f) return;
            //
            // _timer -= Time.deltaTime;
            // if (_timer > 0f) return;
            //
            // _notificationsManager.ShowNotification(Const.Notifications.EvacuationActivated); //$MISSION_COMPLETED_EVACUATION_ACTIVATED
        }

        private void OnEvacuationActivated(ref ActivateEvacuationMessage value)
        {
            IsUsed = false;
            gameObject.SetActive(value.active);
            _notificationsManager.ShowNotification(Const.Notifications.EvacuationActivated); //$MISSION_COMPLETED_EVACUATION_ACTIVATED
            // _timer = TimeToShowEvacuateNotif;
        }

        protected override void OnPlayerEnter()
        {
            var message = new PlayerEvacuatedMessage();
            _messageBroker.Trigger(ref message);
        }

        protected override void OnPlayerStay()
        {
        }

        protected override void OnPlayerExit()
        {
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<ActivateEvacuationMessage>(OnEvacuationActivated);
        }
    }
}