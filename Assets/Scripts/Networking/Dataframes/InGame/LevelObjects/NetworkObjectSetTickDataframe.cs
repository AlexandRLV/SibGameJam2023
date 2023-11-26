using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame.LevelObjects
{
    public struct NetworkObjectSetTickDataframe : INetworkDataframe
    {
        public int objectId;
        public int tick;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(objectId);
            writer.WriteInt(tick);
        }

        public void Read(NetFrameReader reader)
        {
            objectId = reader.ReadInt();
            tick = reader.ReadInt();
        }
    }
}