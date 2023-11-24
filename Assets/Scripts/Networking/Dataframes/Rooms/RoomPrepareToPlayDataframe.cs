using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct RoomPrepareToPlayDataframe : INetworkDataframe
    {
        public bool isMasterClient;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteBool(isMasterClient);
        }

        public void Read(NetFrameReader reader)
        {
            isMasterClient = reader.ReadBool();
        }
    }
}