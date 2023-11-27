using LocalMessages;
using NetFrame;
using Newtonsoft.Json;

namespace Networking
{
    [JsonObject]
    public class DataframeWrapper<T> : IDataframeWrapper where T : struct, INetworkDataframe
    {
        [JsonProperty("d")] public T dataframe;
        
        public void Trigger(LocalMessageBroker broker) => broker.Trigger(ref dataframe);
    }
}