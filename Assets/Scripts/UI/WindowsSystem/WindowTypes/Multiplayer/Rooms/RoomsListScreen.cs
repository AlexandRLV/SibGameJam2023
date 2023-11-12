using System.Collections.Generic;
using Common;
using NetFrame.Client;
using Networking.Dataframes;
using UI.NotificationsSystem;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Multiplayer.Rooms
{
    public class RoomsListScreen : WindowBase
    {
        [Header("Common")]
        [SerializeField] private JoinRoomPopup _joinRoomPopup;
        [SerializeField] private CreateRoomPopup _createRoomPopup;
        [SerializeField] private Button _createRoomButton;
        [SerializeField] private Button _closeButton;
        
        [Header("Rooms list")]
        [SerializeField] private RoomListItem _roomListItemPrefab;
        [SerializeField] private Transform _roomListItemsParent;

        private NetFrameClient _client;
        private NotificationsManager _notificationsManager;
        private WindowsSystem _windowsSystem;
        private List<RoomListItem> _createdRooms;

        private void Awake()
        {
            _createdRooms = new List<RoomListItem>();

            _notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
            _windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            
            _client = GameContainer.Common.Resolve<NetFrameClient>();
            _client.Subscribe<RoomsListDataframe>(SetRooms);
            _client.Subscribe<JoinedRoomDataframe>(ProcessJoinedRoom);
            _client.Subscribe<JoinRoomFailedDataframe>(ProcessJoinFailed);

            var request = new RoomsRequestDataframe();
            _client.Send(ref request);

            _joinRoomPopup.OnJoinPressed += JoinSelectedRoom;
            _joinRoomPopup.OnClosePressed += CloseJoinRoom;

            _createRoomPopup.OnRoomCreated += SendCreateRoom;
            _createRoomPopup.OnClosePressed += CloseCreateRoom;
            
            _createRoomButton.onClick.AddListener(OpenCreateRoom);
            _closeButton.onClick.AddListener(Close);
            
            CloseCreateRoom();
            CloseJoinRoom();
            _roomListItemPrefab.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            var client = GameContainer.Common.Resolve<NetFrameClient>();
            client.Unsubscribe<RoomsListDataframe>(SetRooms);
            client.Unsubscribe<JoinedRoomDataframe>(ProcessJoinedRoom);
            client.Unsubscribe<JoinRoomFailedDataframe>(ProcessJoinFailed);
        }

        private void SetRooms(RoomsListDataframe dataframe)
        {
            var roomsToCreate = ListPool<RoomInfoDataframe>.Get();
            var roomsToDelete = ListPool<RoomListItem>.Get();

            foreach (var room in dataframe.rooms)
            {
                roomsToCreate.Add(room);
            }
            
            foreach (var createdRoom in _createdRooms)
            {
                bool found = false;
                for (int i = roomsToCreate.Count - 1; i >= 0; i--)
                {
                    var roomInfo = roomsToCreate[i];
                    if (roomInfo.roomId != createdRoom.RoomId)
                        continue;

                    createdRoom.SetFromInfo(roomInfo);
                    found = true;
                    roomsToCreate.RemoveAt(i);
                }

                if (!found)
                    roomsToDelete.Add(createdRoom);
            }

            foreach (var room in roomsToDelete)
            {
                if (room == _joinRoomPopup.SelectedRoom)
                    CloseJoinRoom();
                
                Destroy(room.gameObject);
            }

            foreach (var roomInfo in roomsToCreate)
            {
                var room = Instantiate(_roomListItemPrefab, _roomListItemsParent);
                room.gameObject.SetActive(true);
                room.SetFromInfo(roomInfo);
                room.OnRoomSelected = JoinRoom;
            }
        }

        private void JoinSelectedRoom()
        {
            CloseCreateRoom();
            var dataframe = new JoinRoomDataframe
            {
                roomId = _joinRoomPopup.SelectedRoom.RoomId,
                password = _joinRoomPopup.EnteredPassword,
            };
            _client.Send(ref dataframe);
        }

        private void CloseJoinRoom()
        {
            _joinRoomPopup.Clear();
            _joinRoomPopup.gameObject.SetActive(false);
        }

        private void JoinRoom(RoomListItem item)
        {
            _joinRoomPopup.gameObject.SetActive(true);
            _joinRoomPopup.Setup(item);
        }

        private void ProcessJoinedRoom(JoinedRoomDataframe dataframe)
        {
            var currentRoom = _windowsSystem.CreateWindow<CurrentRoomScreen>();
            currentRoom.Setup(dataframe.roomInfo);
            _windowsSystem.DestroyWindow(this);
        }

        private void ProcessJoinFailed(JoinRoomFailedDataframe dataframe)
        {
            string reason = dataframe.reason switch
            {
                JoinRoomFailedReason.WrongPassword => "Неправильный пароль!",
                JoinRoomFailedReason.RoomAlreadyFull => "Комната уже заполнена",
                JoinRoomFailedReason.RoomWasClosed => "Комната была закрыта"
            };

            if (dataframe.reason != JoinRoomFailedReason.WrongPassword)
                CloseJoinRoom();
            
            _notificationsManager.ShowNotification(reason, NotificationsManager.NotificationType.Center);
        }

        private void OpenCreateRoom()
        {
            CloseJoinRoom();
            _createRoomPopup.gameObject.SetActive(true);
        }

        private void SendCreateRoom()
        {
            var dataframe = new CreateRoomDataframe
            {
                name = _createRoomPopup.RoomName,
                password = _createRoomPopup.Password
            };
            _client.Send(ref dataframe);
            CloseCreateRoom();
            _notificationsManager.ShowNotification("Комната создаётся...", NotificationsManager.NotificationType.Center);
        }

        private void CloseCreateRoom()
        {
            _createRoomPopup.gameObject.SetActive(false);
        }

        private void Close()
        {
            _windowsSystem.DestroyWindow(this);
        }
    }
}