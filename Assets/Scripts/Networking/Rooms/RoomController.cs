using Common;
using Common.DI;
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

        public void Initialize()
        {
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