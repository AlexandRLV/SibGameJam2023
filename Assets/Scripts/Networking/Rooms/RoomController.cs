using Common.DI;
using NetFrame.Client;
using Networking.Dataframes;
using Networking.Dataframes.InGame;

namespace Networking
{
    public class RoomController
    {
        public bool inRoom;
        public RoomInfoDataframe currentRoom;
        
        private NetFrameClient _client;

        [Construct]
        public RoomController(NetFrameClient client)
        {
            _client = client;
            _client.Subscribe<JoinedRoomDataframe>(OnJoinedRoom);
            _client.Subscribe<PlayerLeftRoomDataframe>(OnLeftRoom);
            _client.Subscribe<GameFinishedDataframe>(OnGameFinished);
        }

        public void Dispose()
        {
            _client.Unsubscribe<JoinedRoomDataframe>(OnJoinedRoom);
            _client.Unsubscribe<PlayerLeftRoomDataframe>(OnLeftRoom);
            _client.Unsubscribe<GameFinishedDataframe>(OnGameFinished);
        }

        public void LeaveCurrentRoom()
        {
            var dataframe = new LeaveRoomDataframe();
            _client.Send(ref dataframe);
            currentRoom = default;
            inRoom = false;
        }

        private void OnJoinedRoom(JoinedRoomDataframe dataframe)
        {
            currentRoom = dataframe.roomInfo;
            inRoom = true;
        }

        private void OnLeftRoom(PlayerLeftRoomDataframe dataframe)
        {
            currentRoom.guestName = "";
        }
        
        private void OnGameFinished(GameFinishedDataframe dataframe)
        {
            currentRoom.player1Ready = false;
            currentRoom.player2Ready = false;
        }
    }
}