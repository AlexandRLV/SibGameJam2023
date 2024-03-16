using UnityEngine;

namespace UI
{
    public class UIRoot : MonoBehaviour
    {
        public Transform WindowsParent => _windowsParent;
        public Transform NotificationsParent => _notificationsParent;
        public Transform OverlayParent => _overlayParent;

        public bool UiAudioListenerState
        {
            set => _uiListener.enabled = value;
        }

        [SerializeField] private AudioListener _uiListener;
        [SerializeField] private Transform _windowsParent;
        [SerializeField] private Transform _notificationsParent;
        [SerializeField] private Transform _overlayParent;
    }
}