using GameCore.Character.Movement;
using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes.InGame
{
    [JsonObject]
    public struct PlayerEffectStateDataframe : INetworkDataframe
    {
        [JsonIgnore] public EffectType Type => (EffectType)type;
        
        [JsonProperty("t")] public byte type;
        [JsonProperty("a")] public bool active;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte(type);
            writer.WriteBool(active);
        }

        public void Read(NetFrameReader reader)
        {
            type = reader.ReadByte();
            active = reader.ReadBool();
        }
    }
}