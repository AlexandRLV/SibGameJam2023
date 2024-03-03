using Common.DI;
using GameCore.LevelObjects.Abstract;
using UI.NotificationsSystem;
using UnityEngine;

namespace GameCore.LevelObjects.TriggerObjects
{
    public class NotificationTrigger : BaseTriggerObject
    {
        [SerializeField] private string _notificationId;
        
        [Inject] private NotificationsManager _notificationsManager;

        private bool _shown;
        
        private void Start()
        {
            GameContainer.InjectToInstance(this);
        }

        protected override void OnPlayerEnter()
        {
            if (_shown) return;

            _shown = true;
            _notificationsManager.ShowNotification(_notificationId);
        }
    }
}