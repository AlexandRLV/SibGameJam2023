using System;

namespace NetFrame.ThreadSafeContainers
{
    public class DynamicInvokeForServerSafeContainer
    {
        public Delegate Handler;
        public INetworkDataframe Dataframe;
        public int Id;
    }
}