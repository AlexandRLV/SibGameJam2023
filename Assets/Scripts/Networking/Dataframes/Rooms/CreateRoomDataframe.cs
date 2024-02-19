using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.Rooms
{
    [JsonObject]
    public struct CreateRoomDataframe : INetworkDataframe
    {
        [JsonProperty("n")] public string name;
        [JsonProperty("p")] public string password;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteString(name);
            writer.WriteString(password);
        }

        public void Read(NetFrameReader reader)
        {
            name = reader.ReadString();
            password = reader.ReadString();
        }
    }
}