using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes
{
    [JsonObject]
    public struct RoomInfoDataframe : INetworkDataframe
    {
        [JsonProperty("i")] public int roomId;
        [JsonProperty("n")] public string name;
        [JsonProperty("o")] public string ownerName;
        [JsonProperty("g")] public string guestName;
        [JsonProperty("1")] public bool player1Ready;
        [JsonProperty("2")] public bool player2Ready;
        [JsonProperty("p")] public bool hasPassword;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(roomId);
            writer.WriteString(name);
            writer.WriteString(ownerName);
            writer.WriteString(guestName);
            writer.WriteBool(player1Ready);
            writer.WriteBool(player2Ready);
            writer.WriteBool(hasPassword);
        }

        public void Read(NetFrameReader reader)
        {
            roomId = reader.ReadInt();
            name = reader.ReadString();
            ownerName = reader.ReadString();
            guestName = reader.ReadString();
            player1Ready = reader.ReadBool();
            player2Ready = reader.ReadBool();
            hasPassword = reader.ReadBool();
        }
    }
}