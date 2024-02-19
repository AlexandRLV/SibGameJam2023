using System;

namespace Networking.Client.NetFrame
{
    public class SubscriptionContainer
    {
        public Type dataframeType;
        public Delegate handler;
    }
}