using NetFrame;
using NetFrame.WriteAndRead;

namespace SibGameJam.Datagrams
{
    public struct PlayerJoinedDatagram : INetFrameDatagram
    {
        public int ClientId;
        public ClientConnectInfoDatagram Info;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(ClientId);
            writer.Write(Info);
        }

        public void Read(NetFrameReader reader)
        {
            ClientId = reader.ReadInt();
            Info = reader.Read<ClientConnectInfoDatagram>();
        }
    }
}