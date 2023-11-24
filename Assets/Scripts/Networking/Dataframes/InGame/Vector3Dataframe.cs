using NetFrame;
using NetFrame.WriteAndRead;
using UnityEngine;

namespace Networking.Dataframes.InGame
{
    public struct Vector3Dataframe : INetworkDataframe
    {
        public float x;
        public float y;
        public float z;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteFloat(x);
            writer.WriteFloat(y);
            writer.WriteFloat(z);
        }

        public void Read(NetFrameReader reader)
        {
            x = reader.ReadFloat();
            y = reader.ReadFloat();
            z = reader.ReadFloat();
        }

        public static implicit operator Vector3(Vector3Dataframe dataframe) =>
            new(dataframe.x, dataframe.y, dataframe.z);
        
        public static implicit operator Vector3Dataframe(Vector3 vector) =>
            new()
            {
                x = vector.x,
                y = vector.y,
                z = vector.z
            };
    }
}