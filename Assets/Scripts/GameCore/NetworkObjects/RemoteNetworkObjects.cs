using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.NetworkObjects
{
    [Serializable]
    public class RemoteObjectContainer
    {
        [SerializeField] public NetworkObjectType type;
        [SerializeField] public NetworkObject prefab;
    }
    
    [CreateAssetMenu(fileName = "Remote Network Objects")]
    public class RemoteNetworkObjects : ScriptableObject
    {
        [SerializeField] public List<RemoteObjectContainer> remoteObjects;

        public NetworkObject GetPrefabByType(NetworkObjectType type)
        {
            foreach (var container in remoteObjects)
            {
                if (container.type == type)
                    return container.prefab;
            }

            return null;
        }
    }
}