using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;
using UnityEngine;

namespace Networking.Dataframes.InGame
{
    [JsonObject]
    public struct Vector3Dataframe : INetworkDataframe
    {
        [JsonProperty("x")] public float x;
        [JsonProperty("y")] public float y;
        [JsonProperty("z")] public float z;
    
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