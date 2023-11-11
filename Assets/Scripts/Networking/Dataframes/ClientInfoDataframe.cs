using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct ClientInfoDataframe : INetworkDataframe
    {
        public string name;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteString(name);
        }

        public void Read(NetFrameReader reader)
        {
            name = reader.ReadString();
        }
    }
}