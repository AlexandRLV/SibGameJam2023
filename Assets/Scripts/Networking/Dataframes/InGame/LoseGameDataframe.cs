using GameCore.Common;
using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame
{
    public struct LoseGameDataframe : INetworkDataframe
    {
        public LoseGameReason reason;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte) reason);
        }

        public void Read(NetFrameReader reader)
        {
            reason = (LoseGameReason)reader.ReadByte();
        }
    }
}