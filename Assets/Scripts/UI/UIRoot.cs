using UnityEngine;

namespace UI
{
    public class UIRoot : MonoBehaviour
    {
        public Transform WindowsParent => _windowsParent;
        public Transform NotificationsParent => _notificationsParent;
        public Transform OverlayParent => _overlayParent;
        
        [SerializeField] private Transform _windowsParent;
        [SerializeField] private Transform _notificationsParent;
        [SerializeField] private Transform _overlayParent;
    }
}