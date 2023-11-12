using NetFrame;
using NetFrame.WriteAndRead;
using UI.WindowsSystem.WindowTypes.Multiplayer.Rooms;

namespace Networking.Dataframes
{
    public struct JoinRoomFailedDataframe : INetworkDataframe
    {
        public JoinRoomFailedReason reason;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte)reason);
        }

        public void Read(NetFrameReader reader)
        {
            reason = (JoinRoomFailedReason)reader.ReadByte();
        }
    }
}