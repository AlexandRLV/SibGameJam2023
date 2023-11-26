using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame.LevelObjects
{
    public struct DestroyNetworkObjectDataframe : INetworkDataframe
    {
        public int objectId;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(objectId);
        }

        public void Read(NetFrameReader reader)
        {
            objectId = reader.ReadInt();
        }
    }
}