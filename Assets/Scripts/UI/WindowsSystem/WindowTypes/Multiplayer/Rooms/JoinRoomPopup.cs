using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Multiplayer.Rooms
{
    public class JoinRoomPopup : MonoBehaviour
    {
        public Action OnJoinPressed;
        public Action OnClosePressed;

        public RoomListItem SelectedRoom { get; private set; }
        public string EnteredPassword => _passwordInput.text;
        
        [SerializeField] private TextMeshProUGUI _roomNameText;
        [SerializeField] private TextMeshProUGUI _ownerNameText;
        [SerializeField] private GameObject _passwordLabel;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _closeButton;

        private void Awake()
        {
            _joinButton.onClick.AddListener(() => OnJoinPressed?.Invoke());
            _closeButton.onClick.AddListener(() => OnClosePressed?.Invoke());
        }

        public void Setup(RoomListItem item)
        {
            SelectedRoom = item;
            _roomNameText.text = item.Name;
            _ownerNameText.text = item.OwnerName;
            
            _passwordLabel.SetActive(item.HasPassword);
            _passwordInput.gameObject.SetActive(item.HasPassword);
        }

        public void Clear() => SelectedRoom = null;
    }
}