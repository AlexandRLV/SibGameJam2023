using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.Rooms
{
    [JsonObject]
    public struct RoomPrepareToPlayDataframe : INetworkDataframe
    {
        [JsonProperty("m")] public bool isMasterClient;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteBool(isMasterClient);
        }

        public void Read(NetFrameReader reader)
        {
            isMasterClient = reader.ReadBool();
        }
    }
}