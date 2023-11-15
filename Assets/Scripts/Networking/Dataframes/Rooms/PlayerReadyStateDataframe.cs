using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct PlayerReadyStateDataframe : INetworkDataframe
    {
        public int roomId;
        public int playerId;
        public bool ready;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(roomId);
            writer.WriteInt(playerId);
            writer.WriteBool(ready);
        }

        public void Read(NetFrameReader reader)
        {
            roomId = reader.ReadInt();
            playerId = reader.ReadInt();
            ready = reader.ReadBool();
        }
    }
}