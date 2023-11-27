using LocalMessages;

namespace Networking
{
    public interface IDataframeWrapper
    {
        public void Trigger(LocalMessageBroker broker);
    }
}