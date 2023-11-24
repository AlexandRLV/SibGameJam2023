using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public enum DisconnectReason : byte
    {
        ClientVersion,
        NicknameTaken,
    }
    
    public struct DisconnectByReasonDataframe : INetworkDataframe
    {
        public DisconnectReason reason;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte)reason);
        }

        public void Read(NetFrameReader reader)
        {
            reason = (DisconnectReason)reader.ReadByte();
        }
    }
}