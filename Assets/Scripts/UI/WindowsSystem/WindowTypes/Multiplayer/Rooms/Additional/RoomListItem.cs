using System;
using Networking.Dataframes;
using Networking.Dataframes.Rooms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Multiplayer.Rooms
{
    public class RoomListItem : MonoBehaviour
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public bool HasPassword { get; set; }

        public Action<RoomListItem> OnRoomSelected;

        [SerializeField] private TextMeshProUGUI _roomNameText;
        [SerializeField] private TextMeshProUGUI _ownerNameText;
        [SerializeField] private Button _selectRoomButton;

        private void Awake()
        {
            _selectRoomButton.onClick.AddListener(() => OnRoomSelected?.Invoke(this));
        }

        public void SetFromInfo(RoomInfoDataframe dataframe)
        {
            RoomId = dataframe.roomId;
            Name = dataframe.name;
            OwnerName = dataframe.ownerName;
            HasPassword = dataframe.hasPassword;

            _roomNameText.text = dataframe.name;
            _ownerNameText.text = dataframe.ownerName;
        }
    }
}