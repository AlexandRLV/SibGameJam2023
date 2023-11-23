using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame
{
    public struct EnemyDetectPlayerDataframe : INetworkDataframe
    {
        public Vector3Dataframe startPosition;
        public bool isDetect;
        
        public void Write(NetFrameWriter writer)
        {
            writer.Write(startPosition);
            writer.WriteBool(isDetect);
        }

        public void Read(NetFrameReader reader)
        {
            startPosition = reader.Read<Vector3Dataframe>();
            isDetect = reader.ReadBool();
        }
    }
}