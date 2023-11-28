using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UI.WindowsSystem.WindowTypes.Multiplayer.Rooms;

namespace Networking.Dataframes
{
    [JsonObject]
    public struct JoinRoomFailedDataframe : INetworkDataframe
    {
        [JsonProperty("r")] [JsonConverter(typeof(StringEnumConverter))] public JoinRoomFailedReason reason;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte)reason);
        }

        public void Read(NetFrameReader reader)
        {
            reason = (JoinRoomFailedReason)reader.ReadByte();
        }
    }
}