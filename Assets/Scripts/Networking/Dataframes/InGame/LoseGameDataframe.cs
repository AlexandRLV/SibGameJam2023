using GameCore.Common;
using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.InGame
{
    [JsonObject]
    public struct LoseGameDataframe : INetworkDataframe
    {
        public LoseGameReason Reason => (LoseGameReason)reason;
        
        [JsonProperty("r")] public byte reason;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte(reason);
        }

        public void Read(NetFrameReader reader)
        {
            reason = reader.ReadByte();
        }
    }
}