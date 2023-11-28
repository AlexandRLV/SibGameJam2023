using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;
using UnityEngine;

namespace Networking.Dataframes.InGame
{
    [JsonObject]
    public struct PushablePositionDataframe : INetworkDataframe
    {
        [JsonIgnore]
        public Quaternion Rotation
        {
            get => Quaternion.Euler(rotation);
            set => rotation = value.eulerAngles;
        }
        
        [JsonProperty("s")] public Vector3Dataframe startPosition;
        [JsonProperty("p")] public Vector3Dataframe position;
        [JsonProperty("r")] public Vector3Dataframe rotation;
    
        public void Write(NetFrameWriter writer)
        {
            writer.Write(startPosition);
            writer.Write(position);
            writer.Write(rotation);
        }

        public void Read(NetFrameReader reader)
        {
            startPosition = reader.Read<Vector3Dataframe>();
            position = reader.Read<Vector3Dataframe>();
            rotation = reader.Read<Vector3Dataframe>();
        }
    }
}