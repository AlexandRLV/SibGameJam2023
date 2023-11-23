using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame
{
    public struct EnemyDetectPlayerDataframe : INetworkDataframe
    {
        public Vector3Dataframe checkPosition;
        public bool isDetect;
        
        public void Write(NetFrameWriter writer)
        {
            writer.Write(checkPosition);
            writer.WriteBool(isDetect);
        }

        public void Read(NetFrameReader reader)
        {
            checkPosition = reader.Read<Vector3Dataframe>();
            isDetect = reader.ReadBool();
        }
    }
}