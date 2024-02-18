using Newtonsoft.Json;

namespace PlayerProgress
{
    [JsonObject]
    public class ProgressData
    {
        [JsonProperty("completed_level")] public int completedLevel;
    }
}