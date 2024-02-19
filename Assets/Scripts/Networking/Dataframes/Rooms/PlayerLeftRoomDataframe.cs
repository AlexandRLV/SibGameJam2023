using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.Rooms
{
    [JsonObject]
    public struct PlayerLeftRoomDataframe : INetworkDataframe
    {
        public void Write(NetFrameWriter writer)
        {
        }

        public void Read(NetFrameReader reader)
        {
        }
    }
}