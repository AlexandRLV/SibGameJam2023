using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct JoinedRoomDataframe : INetworkDataframe
    {
        public RoomInfoDataframe roomInfo;
        
        public void Write(NetFrameWriter writer)
        {
            writer.Write(roomInfo);
        }

        public void Read(NetFrameReader reader)
        {
            roomInfo = reader.Read<RoomInfoDataframe>();
        }
    }
}