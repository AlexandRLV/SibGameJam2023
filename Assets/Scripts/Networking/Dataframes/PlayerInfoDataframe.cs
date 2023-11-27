using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes
{
    [JsonObject]
    public struct PlayerInfoDataframe : INetworkDataframe
    {
        [JsonProperty("n")] public string name;
        [JsonProperty("c")] public string clientVersion;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteString(name);
            writer.WriteString(clientVersion);
        }

        public void Read(NetFrameReader reader)
        {
            name = reader.ReadString();
            clientVersion = reader.ReadString();
        }
    }
}