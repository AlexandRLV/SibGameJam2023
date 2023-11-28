using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes
{
    [JsonObject]
    public struct JoinedRoomDataframe : INetworkDataframe
    {
        [JsonProperty("i")] public RoomInfoDataframe roomInfo;
        
        public void Write(NetFrameWriter writer)
        {
            writer.Write(roomInfo);
        }

        public void Read(NetFrameReader reader)
        {
            roomInfo = reader.Read<RoomInfoDataframe>();
        }
    }
}