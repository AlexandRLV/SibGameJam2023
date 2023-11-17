using GameCore.Character.Animation;
using GameCore.Player.Network;
using NetFrame;
using NetFrame.WriteAndRead;
using UnityEngine;

namespace Networking.Dataframes.InGame
{
    public struct PlayerPositionDataframe : INetworkDataframe, IInterpolateSnapshot<PlayerPositionDataframe>
    {
        public int Tick { get; set; }
        public bool Teleported { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public AnimationType animationType;
        public float animationSpeed;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(Tick);
            writer.WriteBool(Teleported);
            writer.Write((Vector3Dataframe)Position);
            writer.Write((Vector3Dataframe)Rotation.eulerAngles);
            writer.WriteByte((byte)animationType);
            writer.WriteFloat(animationSpeed);
        }

        public void Read(NetFrameReader reader)
        {
            Tick = reader.ReadInt();
            Teleported = reader.ReadBool();
            Position = reader.Read<Vector3Dataframe>();
            Rotation = Quaternion.Euler(reader.Read<Vector3Dataframe>());
            animationType = (AnimationType)reader.ReadByte();
            animationSpeed = reader.ReadFloat();
        }
        
        public void InterpolateValues(PlayerPositionDataframe from, PlayerPositionDataframe to, float lerpAmount)
        {
            animationType = to.animationType;
            animationSpeed = Mathf.Lerp(from.animationSpeed, to.animationSpeed, lerpAmount);
        }
    }
}