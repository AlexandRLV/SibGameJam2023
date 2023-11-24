using Common.DI;
using NetFrame.Client;
using Networking;
using Networking.Dataframes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Multiplayer.Rooms
{
    public class CurrentRoomWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _roomNameText;
        [SerializeField] private TextMeshProUGUI _player1Text;
        [SerializeField] private TextMeshProUGUI _player2Text;

        [SerializeField] private GameObject _player1ReadyState;
        [SerializeField] private GameObject _player1NotReadyState;
        [SerializeField] private GameObject _player2ReadyState;
        [SerializeField] private GameObject _player2NotReadyState;

        [SerializeField] private GameStartTimerPopup _gameStartTimerPopup;

        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _notReadyButton;
        [SerializeField] private Button _leaveButton;

        [Inject] private RoomController _roomController;
        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private GameClient _gameClient;
        
        private int _localId;

        private void Awake()
        {
            _readyButton.onClick.AddListener(SendReady);
            _notReadyButton.onClick.AddListener(SendNotReady);
            _leaveButton.onClick.AddListener(LeaveRoom);
            
            _gameClient.Client.Subscribe<PlayerLeftRoomDataframe>(OnPlayerLeftRoom);
            _gameClient.Client.Subscribe<PlayerReadyStateDataframe>(OnPlayerReadyStateChanged);
            _gameClient.Client.Subscribe<PlayerJoinedRoomDataframe>(OnPlayerJoinedRoom);
            _gameClient.Client.Subscribe<JoinedRoomDataframe>(OnJoinedRoom);
            _gameClient.Client.Subscribe<RoomPrepareToPlayDataframe>(PrepareToPlay);

            _gameStartTimerPopup.OnTimerEnd += StartGame;
            _gameStartTimerPopup.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _gameClient.Client.Unsubscribe<PlayerLeftRoomDataframe>(OnPlayerLeftRoom);
            _gameClient.Client.Unsubscribe<PlayerReadyStateDataframe>(OnPlayerReadyStateChanged);
            _gameClient.Client.Unsubscribe<PlayerJoinedRoomDataframe>(OnPlayerJoinedRoom);
            _gameClient.Client.Unsubscribe<JoinedRoomDataframe>(OnJoinedRoom);
            _gameClient.Client.Unsubscribe<RoomPrepareToPlayDataframe>(PrepareToPlay);
        }

        public void Setup(RoomInfoDataframe room)
        {
            Debug.Log($"Set up current room, id: {room.roomId}, player1: {room.ownerName}, player2: {room.guestName}");
            _roomNameText.text = room.name;
            _player1Text.text = room.ownerName;
            _player2Text.text = room.guestName;

            if (_gameClient.PlayerName.Equals(room.ownerName))
                _player1Text.text += "(вы)";
            if (_gameClient.PlayerName.Equals(room.guestName))
                _player2Text.text += "(вы)";
            
            _player1ReadyState.SetActive(room.player1Ready);
            _player1NotReadyState.SetActive(!room.player1Ready);
            
            _player2ReadyState.SetActive(!string.IsNullOrWhiteSpace(room.guestName) && room.player2Ready);
            _player2NotReadyState.SetActive(!string.IsNullOrWhiteSpace(room.guestName) && !room.player2Ready);

            bool isLocalOwner = room.ownerName.Equals(_gameClient.PlayerName);
            _localId = isLocalOwner ? 0 : 1;
            
            bool localReady = isLocalOwner ? room.player1Ready : room.player2Ready;
            _readyButton.gameObject.SetActive(!localReady);
            _notReadyButton.gameObject.SetActive(localReady);
        }

        private void SendReady()
        {
            var dataframe = new PlayerReadyStateDataframe
            {
                ready = true,
            };
            _gameClient.Client.Send(ref dataframe);
        }

        private void SendNotReady()
        {
            var dataframe = new PlayerReadyStateDataframe
            {
                ready = false,
            };
            _gameClient.Client.Send(ref dataframe);
        }

        private void LeaveRoom()
        {
            _roomController.LeaveCurrentRoom();

            _windowsSystem.DestroyWindow(this);
            _windowsSystem.CreateWindow<RoomsListWindow>();
        }

        private void OnPlayerReadyStateChanged(PlayerReadyStateDataframe dataframe)
        {
            bool player1 = dataframe.playerId == 0;
            if (player1)
            {
                _player1ReadyState.SetActive(dataframe.ready);
                _player1NotReadyState.SetActive(!dataframe.ready);
            }
            else
            {
                _player2ReadyState.SetActive(dataframe.ready);
                _player2NotReadyState.SetActive(!dataframe.ready);
            }

            if (dataframe.playerId == _localId)
            {
                _readyButton.gameObject.SetActive(!dataframe.ready);
                _notReadyButton.gameObject.SetActive(dataframe.ready);
            }
        }

        private void OnPlayerLeftRoom(PlayerLeftRoomDataframe dataframe)
        {
            _player2Text.text = "";
            _player2ReadyState.SetActive(false);
            _player2NotReadyState.SetActive(false);
            
            _roomController.currentRoom.guestName = "";
        }

        private void OnPlayerJoinedRoom(PlayerJoinedRoomDataframe dataframe)
        {
            _player2Text.text = dataframe.playerName;
            _player2ReadyState.SetActive(false);
            _player2NotReadyState.SetActive(true);
            
            _roomController.currentRoom.guestName = dataframe.playerName;
        }

        private void OnJoinedRoom(JoinedRoomDataframe dataframe)
        {
            Setup(dataframe.roomInfo);
        }

        private void PrepareToPlay(RoomPrepareToPlayDataframe dataframe)
        {
            _gameStartTimerPopup.gameObject.SetActive(true);
            _gameClient.IsMaster = dataframe.isMasterClient;
        }

        private void StartGame()
        {
            _windowsSystem.DestroyWindow(this);
            _windowsSystem.CreateWindow<IntroScreen>();
        }
    }
}