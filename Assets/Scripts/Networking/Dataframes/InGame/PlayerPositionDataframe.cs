using GameCore.Character.Animation;
using GameCore.Player.Network;
using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;
using UnityEngine;

namespace Networking.Dataframes.InGame
{
    [JsonObject]
    public struct PlayerPositionDataframe : INetworkDataframe, IInterpolateSnapshot<PlayerPositionDataframe>
    {
        [JsonIgnore] public AnimationType AnimationType => (AnimationType)animationType;

        [JsonIgnore]
        public Vector3 Position
        {
            get => position;
            set => position = value;
        }
        [JsonIgnore]public Quaternion Rotation
        {
            get => Quaternion.Euler(rotation);
            set => rotation = value.eulerAngles;
        }
        
        [JsonProperty("t")] public int Tick { get; set; }
        [JsonProperty("tp")] public bool Teleported { get; set; }
        [JsonProperty("p")] public Vector3Dataframe position;
        [JsonProperty("r")] public Vector3Dataframe rotation;

        [JsonProperty("at")] public byte animationType;
        [JsonProperty("as")] public float animationSpeed;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(Tick);
            writer.WriteBool(Teleported);
            writer.Write(position);
            writer.Write(rotation);
            writer.WriteByte(animationType);
            writer.WriteFloat(animationSpeed);
        }

        public void Read(NetFrameReader reader)
        {
            Tick = reader.ReadInt();
            Teleported = reader.ReadBool();
            position = reader.Read<Vector3Dataframe>();
            rotation = reader.Read<Vector3Dataframe>();
            animationType = reader.ReadByte();
            animationSpeed = reader.ReadFloat();
        }
        
        public void InterpolateValues(PlayerPositionDataframe from, PlayerPositionDataframe to, float lerpAmount)
        {
            animationType = to.animationType;
            animationSpeed = Mathf.Lerp(from.animationSpeed, to.animationSpeed, lerpAmount);
        }
    }
}