using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct PlayerInfoDataframe : INetworkDataframe
    {
        public int name;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(name);
        }

        public void Read(NetFrameReader reader)
        {
            name = reader.ReadInt();
        }
    }
}