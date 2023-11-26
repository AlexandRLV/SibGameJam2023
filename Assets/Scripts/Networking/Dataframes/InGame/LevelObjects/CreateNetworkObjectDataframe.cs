using GameCore.NetworkObjects;
using NetFrame;
using NetFrame.WriteAndRead;
using UnityEngine;

namespace Networking.Dataframes.InGame.LevelObjects
{
    public struct CreateNetworkObjectDataframe : INetworkDataframe
    {
        public int objectId;
        public NetworkObjectType objectType;
        public Vector3Dataframe position;
        public Quaternion rotation;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(objectId);
            writer.WriteByte((byte)objectType);
            writer.Write(position);
            writer.Write((Vector3Dataframe)rotation.eulerAngles);
        }

        public void Read(NetFrameReader reader)
        {
            objectId = reader.ReadInt();
            objectType = (NetworkObjectType)reader.ReadByte();
            position = reader.Read<Vector3Dataframe>();
            rotation = Quaternion.Euler(reader.Read<Vector3Dataframe>());
        }
    }
}