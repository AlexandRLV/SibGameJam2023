using NetFrame;
using NetFrame.WriteAndRead;

namespace SibGameJam.Datagrams
{
    public struct ClientConnectInfoDatagram : INetFrameDatagram
    {
        public string Name;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteString(Name);
        }

        public void Read(NetFrameReader reader)
        {
            Name = reader.ReadString();
        }
    }
}