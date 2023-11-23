using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame
{
    public struct EnemyAlertPlayerDataframe : INetworkDataframe
    {
        public Vector3Dataframe playerPosition;
        
        public void Write(NetFrameWriter writer)
        {
            writer.Write(playerPosition);
        }

        public void Read(NetFrameReader reader)
        {
            playerPosition = reader.Read<Vector3Dataframe>();
        }
    }
}