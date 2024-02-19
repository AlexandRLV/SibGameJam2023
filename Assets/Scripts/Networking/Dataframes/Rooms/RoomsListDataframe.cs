using System.Collections.Generic;
using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.Rooms
{
    [JsonObject]
    public struct RoomsListDataframe : INetworkDataframe
    {
        [JsonProperty("o")] public int onlinePlayers;
        [JsonProperty("r")] public List<RoomInfoDataframe> rooms;
    
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