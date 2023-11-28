using GameCore.LevelObjects.Abstract;
using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Networking.Dataframes.InGame
{
    [JsonObject]
    public struct InteractedWithObjectDataframe : INetworkDataframe
    {
        [JsonProperty("o")] [JsonConverter(typeof(StringEnumConverter))] public InteractiveObjectType interactedObject;
        [JsonProperty("p")] public Vector3Dataframe objectPosition;
    
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