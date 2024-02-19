using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.Rooms
{
    [JsonObject]
    public struct RoomsRequestDataframe : INetworkDataframe
    {
        public void Write(NetFrameWriter writer)
        {
        }

        public void Read(NetFrameReader reader)
        {
        }
    }
}