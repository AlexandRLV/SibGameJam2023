using Common;
using NetFrame.Client;
using Networking.Dataframes;
using UnityEngine;

namespace Networking
{
    public class RoomController
    {
        public bool inRoom;
        public RoomInfoDataframe currentRoom;
        
        private NetFrameClient _client;
        private GameClient _gameClient;

        public void Initialize()
        {
            _gameClient = GameContainer.Common.Resolve<GameClient>();
            _client = GameContainer.Common.Resolve<NetFrameClient>();
            _client.Subscribe<JoinedRoomDataframe>(OnJoinedRoom);
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
            Debug.Log("Joined room!");
            currentRoom = dataframe.roomInfo;
            inRoom = true;
        }
    }
}