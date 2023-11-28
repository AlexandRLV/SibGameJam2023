using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Networking.Dataframes.InGame
{
    public enum GameFinishedReason : byte
    {
        Win,
        Lose,
        Leave,
        YouLeft,
    }

    [JsonObject]
    public struct GameFinishedDataframe : INetworkDataframe
    {
        [JsonProperty("r")] [JsonConverter(typeof(StringEnumConverter))] public GameFinishedReason reason;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte)reason);
        }

        public void Read(NetFrameReader reader)
        {
            reason = (GameFinishedReason) reader.ReadByte();
        }
    }
}