using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes
{
    [JsonObject]
    public struct PlayerReadyStateDataframe : INetworkDataframe
    {
        [JsonProperty("i")] public int playerId;
        [JsonProperty("r")] public bool ready;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(playerId);
            writer.WriteBool(ready);
        }

        public void Read(NetFrameReader reader)
        {
            playerId = reader.ReadInt();
            ready = reader.ReadBool();
        }
    }
}