using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct RoomInfoDataframe : INetworkDataframe
    {
        public int creatorId;
        public string name;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(creatorId);
            writer.WriteString(name);
        }

        public void Read(NetFrameReader reader)
        {
            creatorId = reader.ReadInt();
            name = reader.ReadString();
        }
    }
}