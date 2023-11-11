using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetFrame.Utils
{
	public static class NetFrameDataframeCollection
	{
		private static readonly Dictionary<string, INetworkDataframe> Dataframes = new();

		public static void Initialize()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var implementingTypes = assembly.GetTypes()
				.Where(t => t.GetInterfaces().Contains(typeof(INetworkDataframe)));

			foreach (var type in implementingTypes)
			{
				if (!type.IsValueType)
				{
					continue;
				}
				
				Dataframes.Add(type.Name, (INetworkDataframe) Activator.CreateInstance(type));
			}
		}

		public static INetworkDataframe GetByKey(string key)
		{
			return Dataframes[key];
		}
	}
}
