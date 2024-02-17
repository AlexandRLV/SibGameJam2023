using Common.DI;
using GameCore.NetworkObjects;
using UnityEngine;

namespace Startup.GameplayInitializers.Common
{
    public class NetworkObjectsInitializer : InitializerBase
    {
        [SerializeField] private RemoteNetworkObjects _remoteNetworkObjects;
        
        public override void Initialize()
        {
            GameContainer.InGame.Register(_remoteNetworkObjects);

            var controller = GameContainer.CreateGameObjectWithComponent<NetworkObjectsController>("NetworkObjectsController");
            GameContainer.InGame.Register(controller);
        }

        public override void Dispose()
        {
        }
    }
}