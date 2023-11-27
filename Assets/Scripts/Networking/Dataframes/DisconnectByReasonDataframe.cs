using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Networking.Dataframes
{
    public enum DisconnectReason : byte
    {
        ClientVersion,
        NicknameTaken,
        ServerError,
    }
    
    [JsonObject]
    public struct DisconnectByReasonDataframe : INetworkDataframe
    {
        [JsonProperty("r")] [JsonConverter(typeof(StringEnumConverter))] public DisconnectReason reason;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte)reason);
        }

        public void Read(NetFrameReader reader)
        {
            reason = (DisconnectReason)reader.ReadByte();
        }
    }
}