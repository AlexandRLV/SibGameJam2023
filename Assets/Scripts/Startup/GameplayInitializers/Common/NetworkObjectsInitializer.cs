using Common.DI;
using GameCore.NetworkObjects;
using UnityEngine;

namespace Startup.GameplayInitializers.Common
{
    public class NetworkObjectsInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var remoteObjects = Resources.Load<RemoteNetworkObjects>("Remote Network Objects");
            GameContainer.InGame.Register(remoteObjects);

            var controllerPrefab = Resources.Load<NetworkObjectsController>("Prefabs/NetworkObjectsController");
            var controller = GameContainer.InstantiateAndResolve(controllerPrefab);
            GameContainer.InGame.Register(controller);
        }

        public override void Dispose()
        {
        }
    }
}