using Common;
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
        
        private NetFrameClient _client;
        private WindowsSystem _windowsSystem;
        private GameClient _gameClient;

        private int _localId;

        private void Awake()
        {
            Debug.Log("Initialize current room window");
            _readyButton.onClick.AddListener(SendReady);
            _notReadyButton.onClick.AddListener(SendNotReady);
            _leaveButton.onClick.AddListener(LeaveRoom);

            _client = GameContainer.Common.Resolve<NetFrameClient>();
            _windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            _gameClient = GameContainer.Common.Resolve<GameClient>();
            
            _client.Subscribe<PlayerLeftRoomDataframe>(OnPlayerLeftRoom);
            _client.Subscribe<PlayerReadyStateDataframe>(OnPlayerReadyStateChanged);
            _client.Subscribe<PlayerJoinedRoomDataframe>(OnPlayerJoinedRoom);
            _client.Subscribe<JoinedRoomDataframe>(OnJoinedRoom);
            _client.Subscribe<RoomPrepareToPlayDataframe>(PrepareToPlay);

            _gameStartTimerPopup.OnTimerEnd += StartGame;
            _gameStartTimerPopup.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Debug.Log("Destroy current room window");
            _client.Unsubscribe<PlayerLeftRoomDataframe>(OnPlayerLeftRoom);
            _client.Unsubscribe<PlayerReadyStateDataframe>(OnPlayerReadyStateChanged);
            _client.Unsubscribe<PlayerJoinedRoomDataframe>(OnPlayerJoinedRoom);
            _client.Unsubscribe<JoinedRoomDataframe>(OnJoinedRoom);
            _client.Unsubscribe<RoomPrepareToPlayDataframe>(PrepareToPlay);
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
            Debug.Log("Sending i'm ready");
            var dataframe = new PlayerReadyStateDataframe
            {
                ready = true,
            };
            _client.Send(ref dataframe);
        }

        private void SendNotReady()
        {
            Debug.Log("Sending i'm not ready");
            var dataframe = new PlayerReadyStateDataframe
            {
                ready = false,
            };
            _client.Send(ref dataframe);
        }

        private void LeaveRoom()
        {
            Debug.Log("Leaving room");
            var roomController = GameContainer.Common.Resolve<RoomController>();
            roomController.LeaveCurrentRoom();

            _windowsSystem.DestroyWindow(this);
            _windowsSystem.CreateWindow<RoomsListWindow>();
        }

        private void OnPlayerReadyStateChanged(PlayerReadyStateDataframe dataframe)
        {
            Debug.Log($"Player ready state changed, id: {dataframe.playerId}, state: {dataframe.ready}");
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
                Debug.Log("Self ready state change callback");
                _readyButton.gameObject.SetActive(!dataframe.ready);
                _notReadyButton.gameObject.SetActive(dataframe.ready);
            }
        }

        private void OnPlayerLeftRoom(PlayerLeftRoomDataframe dataframe)
        {
            Debug.Log("Player left room");
            _player2Text.text = "";
            _player2ReadyState.SetActive(false);
            _player2NotReadyState.SetActive(false);
        }

        private void OnPlayerJoinedRoom(PlayerJoinedRoomDataframe dataframe)
        {
            Debug.Log("Player joined room");
            _player2Text.text = dataframe.playerName;
            _player2ReadyState.SetActive(false);
            _player2NotReadyState.SetActive(true);
        }

        private void OnJoinedRoom(JoinedRoomDataframe dataframe)
        {
            Debug.Log("Self joined room - owner transfership?");
            Setup(dataframe.roomInfo);
        }

        private void PrepareToPlay(RoomPrepareToPlayDataframe dataframe)
        {
            Debug.Log("Game starting in 3 seconds");
            _gameStartTimerPopup.gameObject.SetActive(true);
            _gameClient.IsMaster = dataframe.isMasterClient;
        }

        private void StartGame()
        {
            Debug.Log("Starting game!");
            _windowsSystem.DestroyWindow(this);
            _windowsSystem.CreateWindow<IntroScreen>();
        }
    }
}