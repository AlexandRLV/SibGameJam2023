﻿using NetFrame;
using NetFrame.WriteAndRead;
using Newtonsoft.Json;

namespace Networking.Dataframes
{
    [JsonObject]
    public struct LeaveRoomDataframe : INetworkDataframe
    {
        public void Write(NetFrameWriter writer)
        {
        }

        public void Read(NetFrameReader reader)
        {
        }
    }
}