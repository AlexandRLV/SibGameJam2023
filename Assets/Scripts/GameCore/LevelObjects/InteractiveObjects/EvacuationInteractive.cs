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
            if (_timer <= 0f) return;

            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            
            _notificationsManager.ShowNotification(
                "Все задачи выполнены! Возвращайтесь ко входу для эвакуации.",
                NotificationsManager.NotificationType.Top,
                5f);
        }

        private void OnEvacuationActivated(ref ActivateEvacuationMessage value)
        {
            gameObject.SetActive(value.active);
            _timer = TimeToShowEvacuateNotif;
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