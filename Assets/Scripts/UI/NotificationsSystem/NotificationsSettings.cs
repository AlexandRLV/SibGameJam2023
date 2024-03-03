using System;
using UnityEngine;

namespace UI.NotificationsSystem
{
    [Serializable]
    public class NotificationsSettingsContainer
    {
        [SerializeField] public string id;
        [SerializeField] public string localizationKey;
        [SerializeField] public bool enabled;
        [SerializeField] public NotificationType type;
        [SerializeField] public float showTime;
        [SerializeField] public float maxWaitTime;
        [SerializeField] public float cooldown;
    }
    
    [CreateAssetMenu(fileName = "NotificationsSettings", menuName = "Configs/Notifications Settings")]
    public class NotificationsSettings : ScriptableObject
    {
        [SerializeField] public NotificationsSettingsContainer[] notifications;

        public bool TryGetNotificationById(string id, out NotificationsSettingsContainer container)
        {
            foreach (var notification in notifications)
            {
                if (!notification.id.Equals(id))
                    continue;

                container = notification;
                return true;
            }

            container = null;
            return false;
        }
    }
}