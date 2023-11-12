using System.Collections.Generic;
using Common;
using NetFrame.Client;
using Networking.Dataframes;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes.Multiplayer
{
    public class RoomsListScreen : WindowBase
    {
        [SerializeField] private RoomListItem _roomListItemPrefab;
        [SerializeField] private Transform _roomListItemsParent;

        private List<RoomListItem> _createdRooms;

        private void Awake()
        {
            _createdRooms = new List<RoomListItem>();
            
            var client = GameContainer.Common.Resolve<NetFrameClient>();
            client.Subscribe<RoomsListDataframe>(SetRooms);

            var request = new RoomsRequestDataframe();
            client.Send(ref request);
        }

        private void SetRooms(RoomsListDataframe dataframe)
        {
            foreach (var createdRoom in _createdRooms)
            {
                
            }
        }
    }
}