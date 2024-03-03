using System.Collections.Generic;
using Common.DI;
using Localization;
using UnityEngine;

namespace UI.NotificationsSystem
{
    public class NotificationsManager : MonoBehaviour
    {
        private struct NotificationQueueContainer
        {
            public float requestedTime;
            public List<LocalizationParameter> parameters;
            public NotificationsSettingsContainer settings;
        }

        [SerializeField] private Notification _sideNotification;
        [SerializeField] private Notification _topNotification;
        [SerializeField] private Notification _centerNotification;

        [Inject] private LocalizationProvider _localizationProvider;
        [Inject] private NotificationsSettings _notificationsSettings;
        
        private float _currentShowTimer;
        private Notification _currentNotification;
        private Queue<NotificationQueueContainer> _queuedNotifications;
        private Dictionary<string, float> _lastNotificationsShowTimes;

        private void Start()
        {
            _queuedNotifications = new Queue<NotificationQueueContainer>();
            _lastNotificationsShowTimes = new Dictionary<string, float>();
            
            _sideNotification.gameObject.SetActive(false);
            _topNotification.gameObject.SetActive(false);
            _centerNotification.gameObject.SetActive(false);
        }

        private void Update()
        {
            UpdateCurrentNotification();
            UpdateQueuedNotifications();
        }

        private void UpdateCurrentNotification()
        {
            if (_currentNotification == null) return;
            if (_currentShowTimer <= 0f) return;

            _currentShowTimer -= Time.deltaTime;
            if (_currentShowTimer > 0f) return;
            
            _currentNotification.gameObject.SetActive(false);
            _currentNotification = null;
        }

        private void UpdateQueuedNotifications()
        {
            if (_currentNotification != null) return;
            if (_queuedNotifications.Count == 0) return;

            bool foundNotification = false;
            while (_queuedNotifications.Count > 0 && !foundNotification)
            {
                var queuedNotif = _queuedNotifications.Peek();
                float aliveTime = Time.time - queuedNotif.requestedTime;
                if (aliveTime > queuedNotif.settings.maxWaitTime)
                {
                    _queuedNotifications.Dequeue();
                    continue;
                }

                float lastShowTime = _lastNotificationsShowTimes.GetValueOrDefault(queuedNotif.settings.id, float.MinValue);
                if (Time.time - lastShowTime < queuedNotif.settings.cooldown)
                {
                    _queuedNotifications.Dequeue();
                    continue;
                }

                foundNotification = true;
                ShowNotification(queuedNotif.settings, queuedNotif.parameters);
                _currentShowTimer = queuedNotif.settings.showTime;
            }
        }
        
        public void ShowNotification(string id, List<LocalizationParameter> parameters = null)
        {
            if (!_notificationsSettings.TryGetNotificationById(id, out var notification))
            {
                Debug.LogError($"Не получилось найти конфиг для нотифа {id}! Проверьте id нотифов и конфиг с их настройками");
                return;
            }
            
            if (!notification.enabled)
                return;

            if (_currentNotification == null)
            {
                ShowNotification(notification, parameters);
                return;
            }

            var queueContainer = new NotificationQueueContainer
            {
                requestedTime = Time.time,
                parameters = parameters,
                settings = notification
            };
            _queuedNotifications.Enqueue(queueContainer);
        }

        private void ShowNotification(NotificationsSettingsContainer settings, List<LocalizationParameter> parameters)
        {
            _lastNotificationsShowTimes[settings.id] = Time.time;
            
            string localizedText = _localizationProvider.GetLocalization(settings.localizationKey, parameters);
            
            var notification = settings.type switch
            {
                NotificationType.Side => _sideNotification,
                NotificationType.Center => _centerNotification,
                NotificationType.Top => _topNotification,
                _ => _topNotification
            };
            
            notification.gameObject.SetActive(true);
            notification.Initialize(localizedText);
            _currentShowTimer = settings.showTime;
            _currentNotification = notification;
        }
        
        public void ClearAll()
        {
            if (_currentNotification != null)
            {
                _currentNotification.gameObject.SetActive(false);
                _currentNotification = null;
            }
            
            _currentShowTimer = 0f;
            _queuedNotifications.Clear();
        }

        public void ClearQueue()
        {
            _queuedNotifications.Clear();
        }
    }
}