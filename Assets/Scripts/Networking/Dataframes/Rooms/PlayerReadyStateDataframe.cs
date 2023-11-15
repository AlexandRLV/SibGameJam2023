using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct PlayerReadyStateDataframe : INetworkDataframe
    {
        public int playerId;
        public bool ready;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(playerId);
            writer.WriteBool(ready);
        }

        public void Read(NetFrameReader reader)
        {
            playerId = reader.ReadInt();
            ready = reader.ReadBool();
        }
    }
}