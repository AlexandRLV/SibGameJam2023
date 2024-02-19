using LocalMessages;

namespace Networking.Client.WebSocket
{
    public interface IDataframeWrapper
    {
        public void Trigger(LocalMessageBroker broker);
    }
}