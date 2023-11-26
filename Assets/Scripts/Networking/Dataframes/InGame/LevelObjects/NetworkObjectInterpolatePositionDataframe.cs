using GameCore.Player.Network;
using NetFrame;
using NetFrame.WriteAndRead;
using UnityEngine;

namespace Networking.Dataframes.InGame.LevelObjects
{
    public struct NetworkObjectInterpolatePositionDataframe : INetworkDataframe, IInterpolateSnapshot<NetworkObjectInterpolatePositionDataframe>
    {
        public int objectId;
        public int Tick { get; set; }
        public bool Teleported { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(objectId);
            writer.WriteInt(Tick);
            writer.WriteBool(Teleported);
            writer.Write((Vector3Dataframe)Position);
            writer.Write((Vector3Dataframe)Rotation.eulerAngles);
        }

        public void Read(NetFrameReader reader)
        {
            objectId = reader.ReadInt();
            Tick = reader.ReadInt();
            Teleported = reader.ReadBool();
            Position = reader.Read<Vector3Dataframe>();
            Rotation = Quaternion.Euler(reader.Read<Vector3Dataframe>());
        }

        public void InterpolateValues(NetworkObjectInterpolatePositionDataframe from, NetworkObjectInterpolatePositionDataframe to,
            float lerpAmount)
        {
        }
    }
}