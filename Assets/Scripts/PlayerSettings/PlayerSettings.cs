using Newtonsoft.Json;
using UnityEngine;

namespace PlayerSettings
{
    [JsonObject]
    public class PlayerSettings
    {
        [JsonIgnore]
        public SystemLanguage Language
        {
            get => (SystemLanguage)language;
            set => language = (int)value;
        }
        
        [JsonProperty("invert_x")] public bool invertX;
        [JsonProperty("invert_y")] public bool invertY;
        [JsonProperty("sensitivity")] public float sensitivity;
        [JsonProperty("language")] public int language;
        [JsonProperty("volumes")] public SoundSettings[] soundSettings;
    }
}