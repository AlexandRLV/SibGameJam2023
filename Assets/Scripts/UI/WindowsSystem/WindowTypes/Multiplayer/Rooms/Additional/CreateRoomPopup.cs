using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Multiplayer.Rooms
{
    public class CreateRoomPopup : MonoBehaviour
    {
        public Action OnRoomCreated;
        public Action OnClosePressed;

        public string RoomName => _roomNameText.text;
        public string Password => _passwordText.text;
        
        [SerializeField] private TMP_InputField _roomNameText;
        [SerializeField] private TMP_InputField _passwordText;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _closeButton;

        private void Awake()
        {
            _createButton.onClick.AddListener(() => OnRoomCreated?.Invoke());
            _closeButton.onClick.AddListener(() => OnClosePressed?.Invoke());
        }
    }
}