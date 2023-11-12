using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct PlayerInfoReceivedDataframe : INetworkDataframe
    {
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(0);
        }

        public void Read(NetFrameReader reader)
        {
            reader.ReadInt();
        }
    }
}