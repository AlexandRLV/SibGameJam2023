using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.InGame
{
    [JsonObject]
    public struct EnemyDetectPlayerDataframe : INetworkDataframe
    {
        [JsonProperty("p")] public Vector3Dataframe checkPosition;
        [JsonProperty("d")] public bool isDetect;
        
        public void Write(NetFrameWriter writer)
        {
            writer.Write(checkPosition);
            writer.WriteBool(isDetect);
        }

        public void Read(NetFrameReader reader)
        {
            checkPosition = reader.Read<Vector3Dataframe>();
            isDetect = reader.ReadBool();
        }
    }
}