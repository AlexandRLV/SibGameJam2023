using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame
{
    public struct SetCurrentTickDataframe : INetworkDataframe
    {
        public int tick;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(tick);
        }

        public void Read(NetFrameReader reader)
        {
            tick = reader.ReadInt();
        }
    }
}