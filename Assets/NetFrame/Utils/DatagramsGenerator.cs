using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NetFrame.Utils
{
    public static class DatagramsGenerator
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Run()
        {
            // Find all types in the assembly that implement INetFrameDatagram
            
            var assembly = Assembly.GetExecutingAssembly();
            var implementingTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(INetFrameDatagram)));

            foreach (var type in implementingTypes)
            {
                if (!type.IsValueType) continue;
                NetFrameDatagramCollection.Datagrams.Add(type.Name, (INetFrameDatagram)Activator.CreateInstance(type));
            }
        }
    }
}
