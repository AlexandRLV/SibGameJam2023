using NetFrame;
using NetFrame.WriteAndRead;
using UnityEngine;

namespace Networking.Dataframes.InGame
{
    public struct PushablePositionDataframe : INetworkDataframe
    {
        public Vector3Dataframe startPosition;
        public Vector3Dataframe position;
        public Quaternion rotation;
    
        public void Write(NetFrameWriter writer)
        {
            writer.Write(startPosition);
            writer.Write(position);
            writer.Write((Vector3Dataframe)rotation.eulerAngles);
        }

        public void Read(NetFrameReader reader)
        {
            startPosition = reader.Read<Vector3Dataframe>();
            position = reader.Read<Vector3Dataframe>();
            rotation = Quaternion.Euler(reader.Read<Vector3Dataframe>());
        }
    }
}