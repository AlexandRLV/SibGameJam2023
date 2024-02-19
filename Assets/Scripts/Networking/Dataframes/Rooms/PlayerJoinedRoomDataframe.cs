using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.Rooms
{
    [JsonObject]
    public struct PlayerJoinedRoomDataframe : INetworkDataframe
    {
        [JsonProperty("i")] public int playerId;
        [JsonProperty("n")] public string playerName;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(playerId);
            writer.WriteString(playerName);
        }

        public void Read(NetFrameReader reader)
        {
            playerId = reader.ReadInt();
            playerName = reader.ReadString();
        }
    }
}