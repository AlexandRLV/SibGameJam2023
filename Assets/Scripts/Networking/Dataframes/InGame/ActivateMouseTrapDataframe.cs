using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.InGame
{
    [JsonObject]
    public struct ActivateMouseTrapDataframe : INetworkDataframe
    {
        [JsonProperty("p")] public Vector3Dataframe mousetrapPosition;
        
        public void Write(NetFrameWriter writer)
        {
            writer.Write(mousetrapPosition);
        }

        public void Read(NetFrameReader reader)
        {
            mousetrapPosition = reader.Read<Vector3Dataframe>();
        }
    }
}