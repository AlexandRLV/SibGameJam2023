using System.Collections.Generic;
using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct RoomsListDataframe : INetworkDataframe
    {
        public int onlinePlayers;
        public List<RoomInfoDataframe> rooms;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(onlinePlayers);
            writer.WriteInt(rooms.Count);
            foreach (var room in rooms)
            {
                writer.Write(room);
            }
        }

        public void Read(NetFrameReader reader)
        {
            onlinePlayers = reader.ReadInt();
            int count = reader.ReadInt();
            rooms = new List<RoomInfoDataframe>();
            for (int i = 0; i < count; i++)
            {
                rooms.Add(reader.Read<RoomInfoDataframe>());
            }
        }
    }
}