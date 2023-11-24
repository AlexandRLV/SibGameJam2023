using GameCore.Character.Movement;
using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame
{
    public struct PlayerEffectStateDataframe : INetworkDataframe
    {
        public EffectType type;
        public bool active;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte)type);
            writer.WriteBool(active);
        }

        public void Read(NetFrameReader reader)
        {
            type = (EffectType)reader.ReadByte();
            active = reader.ReadBool();
        }
    }
}