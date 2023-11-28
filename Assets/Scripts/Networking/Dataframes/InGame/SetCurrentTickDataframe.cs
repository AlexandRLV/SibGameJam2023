using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.InGame
{
    [JsonObject]
    public struct SetCurrentTickDataframe : INetworkDataframe
    {
        [JsonProperty("t")] public int tick;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(tick);
        }

        public void Read(NetFrameReader reader)
        {
            tick = reader.ReadInt();
        }
    }
}