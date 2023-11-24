using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct PlayerInfoDataframe : INetworkDataframe
    {
        public string name;
        public string clientVersion;
        
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