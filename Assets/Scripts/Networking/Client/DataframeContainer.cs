using Newtonsoft.Json;

namespace Networking
{
    [JsonObject]
    public struct DataframeContainer
    {
        [JsonProperty("t")] public string dataframeType;
        [JsonProperty("c")] public string contents;
    }
}