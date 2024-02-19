using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.Rooms
{
    [JsonObject]
    public struct JoinRoomRequestDataframe : INetworkDataframe
    {
        [JsonProperty("i")] public int roomId;
        [JsonProperty("p")] public string password;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(roomId);
            writer.WriteString(password);
        }

        public void Read(NetFrameReader reader)
        {
            roomId = reader.ReadInt();
            password = reader.ReadString();
        }
    }
}