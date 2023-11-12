using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct RoomInfoDataframe : INetworkDataframe
    {
        public int roomId;
        public string name;
        public string ownerName;
        public bool hasPassword;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(roomId);
            writer.WriteString(name);
            writer.WriteString(ownerName);
            writer.WriteBool(hasPassword);
        }

        public void Read(NetFrameReader reader)
        {
            roomId = reader.ReadInt();
            name = reader.ReadString();
            ownerName = reader.ReadString();
            hasPassword = reader.ReadBool();
        }
    }
}