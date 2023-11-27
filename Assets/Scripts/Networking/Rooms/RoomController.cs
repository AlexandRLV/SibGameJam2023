using Common.DI;
using LocalMessages;
using NetFrame.Client;
using Networking.Dataframes;
using Networking.Dataframes.InGame;

namespace Networking
{
    public class RoomController
    {
        public bool inRoom;
        public RoomInfoDataframe currentRoom;
        
        private IGameClient _client;
        private LocalMessageBroker _messageBroker;

        [Construct]
        public RoomController(IGameClient client, LocalMessageBroker messageBroker)
        {
            _client = client;
            _messageBroker = messageBroker;
            _messageBroker.Subscribe<JoinedRoomDataframe>(OnJoinedRoom);
            _messageBroker.Subscribe<PlayerLeftRoomDataframe>(OnLeftRoom);
            _messageBroker.Subscribe<GameFinishedDataframe>(OnGameFinished);
        }

        public void Dispose()
        {
            _messageBroker.Unsubscribe<JoinedRoomDataframe>(OnJoinedRoom);
            _messageBroker.Unsubscribe<PlayerLeftRoomDataframe>(OnLeftRoom);
            _messageBroker.Unsubscribe<GameFinishedDataframe>(OnGameFinished);
        }

        public void LeaveCurrentRoom()
        {
            var dataframe = new LeaveRoomDataframe();
            _client.Send(ref dataframe);
            currentRoom = default;
            inRoom = false;
        }

        private void OnJoinedRoom(ref JoinedRoomDataframe dataframe)
        {
            currentRoom = dataframe.roomInfo;
            inRoom = true;
        }

        private void OnLeftRoom(ref PlayerLeftRoomDataframe dataframe)
        {
            currentRoom.guestName = "";
        }
        
        private void OnGameFinished(ref GameFinishedDataframe dataframe)
        {
            currentRoom.player1Ready = false;
            currentRoom.player2Ready = false;
        }
    }
}