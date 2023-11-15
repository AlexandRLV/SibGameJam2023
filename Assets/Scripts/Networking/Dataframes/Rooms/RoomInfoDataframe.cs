using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct RoomInfoDataframe : INetworkDataframe
    {
        public int roomId;
        public string name;
        public string ownerName;
        public string guestName;
        public bool player1Ready;
        public bool player2Ready;
        public bool hasPassword;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(roomId);
            writer.WriteString(name);
            writer.WriteString(ownerName);
            writer.WriteString(guestName);
            writer.WriteBool(player1Ready);
            writer.WriteBool(player2Ready);
            writer.WriteBool(hasPassword);
        }

        public void Read(NetFrameReader reader)
        {
            roomId = reader.ReadInt();
            name = reader.ReadString();
            ownerName = reader.ReadString();
            guestName = reader.ReadString();
            player1Ready = reader.ReadBool();
            player2Ready = reader.ReadBool();
            hasPassword = reader.ReadBool();
        }
    }
}