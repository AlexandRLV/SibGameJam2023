using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct LeaveRoomDataframe : INetworkDataframe
    {
        public int roomId;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(roomId);
        }

        public void Read(NetFrameReader reader)
        {
            roomId = reader.ReadInt();
        }
    }
}