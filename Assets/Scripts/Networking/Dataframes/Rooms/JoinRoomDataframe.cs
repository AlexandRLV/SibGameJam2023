using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct JoinRoomDataframe : INetworkDataframe
    {
        public int roomId;
        public string password;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(roomId);
            writer.WriteString(password);
        }

        public void Read(NetFrameReader reader)
        {
            roomId = reader.ReadInt();
            password = reader.ReadString();
        }
    }
}