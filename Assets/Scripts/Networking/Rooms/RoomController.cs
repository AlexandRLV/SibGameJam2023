using Common.DI;
using NetFrame.Client;
using Networking.Dataframes;

namespace Networking
{
    public class RoomController
    {
        public bool inRoom;
        public RoomInfoDataframe currentRoom;
        
        private NetFrameClient _client;

        [Inject]
        public RoomController(NetFrameClient client)
        {
            _client = client;
            _client.Subscribe<JoinedRoomDataframe>(OnJoinedRoom);
            _client.Subscribe<PlayerLeftRoomDataframe>(OnLeftRoom);
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
    }
}