using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes.InGame
{
    public struct ActivateMouseTrapDataframe : INetworkDataframe
    {
        public Vector3Dataframe mousetrapPosition;
        
        public void Write(NetFrameWriter writer)
        {
            writer.Write(mousetrapPosition);
        }

        public void Read(NetFrameReader reader)
        {
            mousetrapPosition = reader.Read<Vector3Dataframe>();
        }
    }
}