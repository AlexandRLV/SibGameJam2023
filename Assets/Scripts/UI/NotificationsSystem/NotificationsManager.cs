using UnityEngine;

namespace UI.NotificationsSystem
{
    public class NotificationsManager : MonoBehaviour
    {
        public enum NotificationType
        {
            Top,
            Center,
            Side,
        }

        [SerializeField] private float _showTime;
        [SerializeField] private NotificationsPool _centerNotificationsPool;
        [SerializeField] private NotificationsPool _topNotificationsPool;
     
        [SerializeField] private RectTransform _topNotificationsParent;
        [SerializeField] private RectTransform _centerNotificationsParent;

        [SerializeField] private Notification _sideNotification;

        private void Start()
        {
            _sideNotification.gameObject.SetActive(false);
        }

        public void ShowNotification(string text, NotificationType type, float time = -1)
        {
            var notification = GetNotificationForType(type);
            notification.gameObject.SetActive(true);

            float showTime = time > 0f ? time : _showTime;
            notification.Initialize(text, showTime);
        }

        private Notification GetNotificationForType(NotificationType type)
        {
            if (type == NotificationType.Side) return _sideNotification;
            
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
            
            var notification = pool.GetForTime(_showTime);
            notification.transform.SetParent(parent);
            return notification;
        }
    }
}