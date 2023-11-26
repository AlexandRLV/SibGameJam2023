using UnityEngine;

namespace UI.NotificationsSystem
{
    public class NotificationsManager : MonoBehaviour
    {
        public enum NotificationType
        {
            Top,
            Center,
        }

        [SerializeField] private float _showTime;
        [SerializeField] private NotificationsPool _centerNotificationsPool;
        [SerializeField] private NotificationsPool _topNotificationsPool;
     
        [SerializeField] private RectTransform _topNotificationsParent;
        [SerializeField] private RectTransform _centerNotificationsParent;

        public void ShowNotification(string text, NotificationType type, float time = -1)
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
            
            var notification = pool.GetForTime(_showTime);
            notification.gameObject.SetActive(true);
            notification.transform.SetParent(parent);

            float showTime = time > 0f ? time : _showTime;
            notification.Initialize(text, showTime);
        }
    }
}