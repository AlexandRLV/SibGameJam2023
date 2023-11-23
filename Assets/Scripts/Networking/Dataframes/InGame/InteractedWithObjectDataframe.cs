using GameCore.InteractiveObjects;
using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame
{
    public struct InteractedWithObjectDataframe : INetworkDataframe
    {
        public InteractiveObjectType interactedObject;
        public Vector3Dataframe objectPosition;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte)interactedObject);
            writer.Write(objectPosition);
        }

        public void Read(NetFrameReader reader)
        {
            interactedObject = (InteractiveObjectType) reader.ReadByte();
            objectPosition = reader.Read<Vector3Dataframe>();
        }
    }
}