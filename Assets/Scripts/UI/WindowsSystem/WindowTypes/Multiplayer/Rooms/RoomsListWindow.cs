using System.Collections.Generic;
using Common;
using NetFrame.Client;
using Networking;
using Networking.Dataframes;
using Startup;
using UI.NotificationsSystem;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Multiplayer.Rooms
{
    public class RoomsListWindow : WindowBase
    {
        [Header("Common")]
        [SerializeField] private JoinRoomPopup _joinRoomPopup;
        [SerializeField] private CreateRoomPopup _createRoomPopup;
        [SerializeField] private Button _createRoomButton;
        [SerializeField] private Button _closeButton;
        
        [Header("Rooms list")]
        [SerializeField] private RoomListItem _roomListItemPrefab;
        [SerializeField] private Transform _roomListItemsParent;

        private float _roomsRequestTimer;

        private NetFrameClient _client;
        private NotificationsManager _notificationsManager;
        private WindowsSystem _windowsSystem;
        private ClientParameters _clientParameters;
        
        private List<RoomListItem> _createdRooms;

        private void Awake()
        {
            Debug.Log("Room list window initialize");
            _createdRooms = new List<RoomListItem>();

            _notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
            _windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            
            _client = GameContainer.Common.Resolve<NetFrameClient>();
            _client.Subscribe<RoomsListDataframe>(SetRooms);
            _client.Subscribe<JoinedRoomDataframe>(ProcessJoinedRoom);
            _client.Subscribe<JoinRoomFailedDataframe>(ProcessJoinFailed);

            _joinRoomPopup.OnJoinPressed += JoinSelectedRoom;
            _joinRoomPopup.OnClosePressed += CloseJoinRoom;

            _createRoomPopup.OnRoomCreated += SendCreateRoom;
            _createRoomPopup.OnClosePressed += CloseCreateRoom;
            
            _createRoomButton.onClick.AddListener(OpenCreateRoom);
            _closeButton.onClick.AddListener(Close);
            
            CloseCreateRoom();
            CloseJoinRoom();
            _roomListItemPrefab.gameObject.SetActive(false);

            _clientParameters = GameContainer.Common.Resolve<ClientParameters>();

            var roomController = GameContainer.Common.Resolve<RoomController>();
            if (!roomController.inRoom) return;
            
            Debug.Log("Already in room!");
            var currentRoom = _windowsSystem.CreateWindow<CurrentRoomWindow>();
            currentRoom.Setup(roomController.currentRoom);
            _windowsSystem.DestroyWindow(this);
        }

        private void Update()
        {
            _roomsRequestTimer -= Time.deltaTime;
            if (_roomsRequestTimer > 0f) return;
            
            var request = new RoomsRequestDataframe();
            _client.Send(ref request);
            _roomsRequestTimer = _clientParameters.roomsRequestInterval;
        }

        private void OnDestroy()
        {
            Debug.Log("Destroy room list window");
            _client.Unsubscribe<RoomsListDataframe>(SetRooms);
            _client.Unsubscribe<JoinedRoomDataframe>(ProcessJoinedRoom);
            _client.Unsubscribe<JoinRoomFailedDataframe>(ProcessJoinFailed);
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
                
                _createdRooms.Remove(room);
                Destroy(room.gameObject);
            }

            foreach (var roomInfo in roomsToCreate)
            {
                var room = Instantiate(_roomListItemPrefab, _roomListItemsParent);
                room.gameObject.SetActive(true);
                room.SetFromInfo(roomInfo);
                room.OnRoomSelected = JoinRoom;
                _createdRooms.Add(room);
            }
            
            ListPool<RoomInfoDataframe>.Release(roomsToCreate);
            ListPool<RoomListItem>.Release(roomsToDelete);
        }

        private void JoinSelectedRoom()
        {
            Debug.Log("Sending request to join room");
            CloseCreateRoom();
            var dataframe = new JoinRoomRequestDataframe
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
            Debug.Log("Joined room, switching to current room window");
            var currentRoom = _windowsSystem.CreateWindow<CurrentRoomWindow>();
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
            
            Debug.Log($"Join room failed: {reason}");
            
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
            if (string.IsNullOrWhiteSpace(_createRoomPopup.RoomName))
            {
                var notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
                notificationsManager.ShowNotification("Введите имя комнаты!", NotificationsManager.NotificationType.Center);
                return;
            }
            
            Debug.Log("Sending create room");
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
            Debug.Log("Disconnecting");
            var gameClient = GameContainer.Common.Resolve<GameClient>();
            gameClient.Disconnect();
            
            _windowsSystem.DestroyWindow(this);
            _windowsSystem.CreateWindow<ConnectWindow>();
        }
    }
}