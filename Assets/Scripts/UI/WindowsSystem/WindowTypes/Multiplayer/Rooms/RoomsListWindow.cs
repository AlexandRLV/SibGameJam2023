using System.Collections.Generic;
using Common.DI;
using NetFrame.Client;
using Networking;
using Networking.Dataframes;
using Startup;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI _playersOnlineText;
        
        [Header("Rooms list")]
        [SerializeField] private RoomListItem _roomListItemPrefab;
        [SerializeField] private Transform _roomListItemsParent;

        [Inject] private NotificationsManager _notificationsManager;
        [Inject] private ClientParameters _clientParameters;
        [Inject] private RoomController _roomController;
        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private GameClient _gameClient;
        
        private float _roomsRequestTimer;
        private List<RoomListItem> _createdRooms;

        private void Awake()
        {
            Debug.Log("Room list window initialize");
            _createdRooms = new List<RoomListItem>();

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

        private void Start()
        {
            _gameClient.Client.Subscribe<RoomsListDataframe>(SetRooms);
            _gameClient.Client.Subscribe<JoinedRoomDataframe>(ProcessJoinedRoom);
            _gameClient.Client.Subscribe<JoinRoomFailedDataframe>(ProcessJoinFailed);
            
            if (!_roomController.inRoom) return;
            
            _windowsSystem.DestroyWindow(this);
            var currentRoom = _windowsSystem.CreateWindow<CurrentRoomWindow>();
            currentRoom.Setup(_roomController.currentRoom);
        }

        private void Update()
        {
            _roomsRequestTimer -= Time.deltaTime;
            if (_roomsRequestTimer > 0f) return;
            
            var request = new RoomsRequestDataframe();
            _gameClient.Client.Send(ref request);
            _roomsRequestTimer = _clientParameters.roomsRequestInterval;
        }

        private void OnDestroy()
        {
            _gameClient.Client.Unsubscribe<RoomsListDataframe>(SetRooms);
            _gameClient.Client.Unsubscribe<JoinedRoomDataframe>(ProcessJoinedRoom);
            _gameClient.Client.Unsubscribe<JoinRoomFailedDataframe>(ProcessJoinFailed);
        }

        private void SetRooms(RoomsListDataframe dataframe)
        {
            _playersOnlineText.text = dataframe.onlinePlayers.ToString();
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
            _gameClient.Client.Send(ref dataframe);
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
            _windowsSystem.DestroyWindow(this);
            var currentRoom = _windowsSystem.CreateWindow<CurrentRoomWindow>();
            currentRoom.Setup(dataframe.roomInfo);
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
                _notificationsManager.ShowNotification("Введите имя комнаты!", NotificationsManager.NotificationType.Center);
                return;
            }
            
            Debug.Log("Sending create room");
            var dataframe = new CreateRoomDataframe
            {
                name = _createRoomPopup.RoomName,
                password = _createRoomPopup.Password
            };
            _gameClient.Client.Send(ref dataframe);
            CloseCreateRoom();
            _notificationsManager.ShowNotification("Комната создаётся...", NotificationsManager.NotificationType.Center, 0.5f);
        }

        private void CloseCreateRoom()
        {
            _createRoomPopup.gameObject.SetActive(false);
        }

        private void Close()
        {
            Debug.Log("Disconnecting");
            _gameClient.Disconnect();
            
            _windowsSystem.DestroyWindow(this);
            _windowsSystem.CreateWindow<ConnectWindow>();
        }
    }
}