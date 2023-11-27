using NetFrame;

namespace Networking
{
    public interface IGameClient
    {
        public void Connect();
        public void Disconnect();
        
        public void Send<T>(ref T dataframe) where T : struct, INetworkDataframe;
    }
}