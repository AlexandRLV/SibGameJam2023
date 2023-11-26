using System.Collections;
using Common.DI;
using GameCore.NetworkObjects;
using UnityEngine;

namespace Startup.GameplayInitializers.Multiplayer
{
    public class NetworkObjectsInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var remoteObjects = Resources.Load<RemoteNetworkObjects>("Remote Network Objects");
            GameContainer.InGame.Register(remoteObjects);

            var controllerPrefab = Resources.Load<NetworkObjectsController>("Prefabs/NetworkObjectsController");
            var controller = GameContainer.InstantiateAndResolve(controllerPrefab);
            GameContainer.InGame.Register(controller);
            
            yield break;
        }

        public void Dispose()
        {
        }
    }
}