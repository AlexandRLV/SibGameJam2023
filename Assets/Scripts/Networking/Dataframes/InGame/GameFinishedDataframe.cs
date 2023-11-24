using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame
{
    public enum GameFinishedReason : byte
    {
        Win,
        Lose,
        Leave,
        YouLeft,
    }

    public struct GameFinishedDataframe : INetworkDataframe
    {
        public GameFinishedReason reason;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte)reason);
        }

        public void Read(NetFrameReader reader)
        {
            reason = (GameFinishedReason) reader.ReadByte();
        }
    }
}