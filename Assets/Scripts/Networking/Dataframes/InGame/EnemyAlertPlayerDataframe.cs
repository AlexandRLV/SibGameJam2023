using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.InGame
{
    [JsonObject]
    public struct EnemyAlertPlayerDataframe : INetworkDataframe
    {
        [JsonProperty("p")] public Vector3Dataframe playerPosition;
        
        public void Write(NetFrameWriter writer)
        {
            writer.Write(playerPosition);
        }

        public void Read(NetFrameReader reader)
        {
            playerPosition = reader.Read<Vector3Dataframe>();
        }
    }
}