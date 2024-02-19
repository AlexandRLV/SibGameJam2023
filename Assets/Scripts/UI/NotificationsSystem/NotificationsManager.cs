using System.Collections.Generic;
using Common.DI;
using Localization;
using UnityEngine;

namespace UI.NotificationsSystem
{
    public class NotificationsManager : MonoBehaviour
    {
        private struct NotificationContainer
        {
            public Notification notification;
            public float showTimer;
            public NotificationsPool pool;
        }

        [SerializeField] private float _showTime;
        [SerializeField] private NotificationsPool _centerNotificationsPool;
        [SerializeField] private NotificationsPool _topNotificationsPool;
     
        [SerializeField] private RectTransform _topNotificationsParent;
        [SerializeField] private RectTransform _centerNotificationsParent;

        [SerializeField] private Notification _sideNotification;

        [Inject] private LocalizationProvider _localizationProvider;
        
        private float _sideTimer;
        private List<NotificationContainer> _activeNotifications;

        private void Start()
        {
            _activeNotifications = new List<NotificationContainer>();
            _sideNotification.gameObject.SetActive(false);
        }

        private void Update()
        {
            for (int i = _activeNotifications.Count - 1; i >= 0; i--)
            {
                var container = _activeNotifications[i];
                container.showTimer -= Time.deltaTime;
                if (container.showTimer > 0f) continue;
                
                container.pool.Return(container.notification);
                _activeNotifications.RemoveAt(i);
            }
            
            if (_sideTimer <= 0f) return;

            _sideTimer -= Time.deltaTime;
            if (_sideTimer > 0f) return;
            
            _sideNotification.gameObject.SetActive(false);
        }

        public void ShowNotification(string localizationKey, NotificationType type, float time = -1, List<LocalizationParameter> parameters = null)
        {
            string localizedText = _localizationProvider.GetLocalization(localizationKey, parameters);
            
            float showTime = time > 0f ? time : _showTime;
            if (type == NotificationType.Side)
            {
                _sideNotification.gameObject.SetActive(true);
                _sideNotification.Initialize(localizedText);
                _sideTimer = showTime;
                return;
            }
            
            var container = GetNotificationForType(type);
            container.notification.Initialize(localizedText);
            container.showTimer = showTime;
            _activeNotifications.Add(container);
        }

        public void ClearAll()
        {
            _sideNotification.gameObject.SetActive(false);
            for (int i = _activeNotifications.Count - 1; i >= 0; i--)
            {
                var container = _activeNotifications[i];
                container.pool.Return(container.notification);
            }
            
            _activeNotifications.Clear();
        }
        
        private NotificationContainer GetNotificationForType(NotificationType type)
        {
            var parent = type switch
            {
                NotificationType.Center => _centerNotificationsParent,
                _ => _topNotificationsParent
            };

            var pool = type switch
            {
                NotificationType.Center => _centerNotificationsPool,
                _ => _topNotificationsPool
            };
            
            var notification = pool.Get();
            notification.transform.SetParent(parent);
            notification.gameObject.SetActive(true);
            
            return new NotificationContainer
            {
                notification = notification,
                pool = pool,
            };
        }
    }
}