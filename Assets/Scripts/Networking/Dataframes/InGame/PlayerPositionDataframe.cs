using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame
{
    public struct PlayerPositionDataframe : INetworkDataframe
    {
        public int tick;
        public Vector3Dataframe position;
        public Vector3Dataframe rotation;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(tick);
            writer.Write(position);
            writer.Write(rotation);
        }

        public void Read(NetFrameReader reader)
        {
            tick = reader.ReadInt();
            position = reader.Read<Vector3Dataframe>();
            rotation = reader.Read<Vector3Dataframe>();
        }
    }
}