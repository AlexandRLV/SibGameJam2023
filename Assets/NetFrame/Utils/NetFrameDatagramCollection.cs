using System.Collections.Generic;

namespace NetFrame.Utils
{
	public class NetFrameDatagramCollection
	{
		public Dictionary<string, INetFrameDatagram> _datagrams;

		public NetFrameDatagramCollection()
		{
			_datagrams = new Dictionary<string, INetFrameDatagram>();
		}

		public INetFrameDatagram GetDatagramByKey(string key)
		{
			return _datagrams[key];
		}
	}
}
