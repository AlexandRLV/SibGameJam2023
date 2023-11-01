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
        [SerializeField] private NotificationsPool _notificationsPool;
     
        [SerializeField] private Transform _topNotificationsParent;
        [SerializeField] private Transform _centerNotificationsParent;
        [SerializeField] private Transform _sideNotificationsParent;

        public void ShowNotification(string text, NotificationType type)
        {
            var parent = type switch
            {
                NotificationType.Top => _topNotificationsParent,
                NotificationType.Center => _centerNotificationsParent,
                NotificationType.Side => _sideNotificationsParent
            };
            var notification = _notificationsPool.GetForTime(_showTime);
            notification.gameObject.SetActive(true);
            notification.transform.parent = parent;
            notification.Initialize(text, _showTime);
        }
    }
}