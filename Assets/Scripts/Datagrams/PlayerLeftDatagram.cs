using NetFrame;
using NetFrame.WriteAndRead;

namespace SibGameJam.Datagrams
{
    public struct PlayerLeftDatagram : INetFrameDatagram
    {
        public int Id;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(Id);
        }

        public void Read(NetFrameReader reader)
        {
            Id = reader.ReadInt();
        }
    }
}