using Newtonsoft.Json;

namespace Networking.Client.WebSocket
{
    [JsonObject]
    public struct DataframeContainer
    {
        [JsonProperty("t")] public string dataframeType;
        [JsonProperty("c")] public string contents;
    }
}