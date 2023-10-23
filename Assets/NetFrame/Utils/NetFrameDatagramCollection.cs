using System.Collections.Generic;

using SibGameJam.Datagrams;

namespace NetFrame.Utils
{
	public static class NetFrameDatagramCollection
	{
		public static Dictionary<string, INetFrameDatagram> Datagrams = new();

		public static INetFrameDatagram GetDatagramByKey(string key)
		{
			return Datagrams[key];
		}
	}
}
