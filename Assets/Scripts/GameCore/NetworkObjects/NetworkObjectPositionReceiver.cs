using Common.DI;
using LocalMessages;
using Networking;
using Networking.Dataframes.InGame.LevelObjects;
using UnityEngine;

namespace GameCore.NetworkObjects
{
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class NetworkObjectPositionReceiver : MonoBehaviour
    {
        [SerializeField] private NetworkObject _networkObject;
        [SerializeField] private NetworkObjectPositionInterpolator _interpolator;

        [Inject] private LocalMessageBroker _messageBroker;
        [Inject] private IGameClient _gameClient;

        private bool _subscribed;
        
        private void Start()
        {
            if (!_networkObject.IsOnline)
                return;
            
            _messageBroker.Subscribe<NetworkObjectSetTickDataframe>(SetTick);
            _messageBroker.Subscribe<NetworkObjectInterpolatePositionDataframe>(PushPositionSnapshot);
            _subscribed = true;
        }

        private void OnDestroy()
        {
            if (!_subscribed)
                return;

            _messageBroker.Unsubscribe<NetworkObjectSetTickDataframe>(SetTick);
            _messageBroker.Unsubscribe<NetworkObjectInterpolatePositionDataframe>(PushPositionSnapshot);
        }

        private void Update()
        {
            ref var snapshot = ref _interpolator.Current;
            transform.SetPositionAndRotation(snapshot.Position, snapshot.Rotation);
        }

        private void SetTick(ref NetworkObjectSetTickDataframe dataframe)
        {
            _interpolator.SetOwnerTick(dataframe.tick);
        }

        private void PushPositionSnapshot(ref NetworkObjectInterpolatePositionDataframe dataframe)
        {
            if (dataframe.objectId != _networkObject.Id)
                return;
            
            _interpolator.AddSnapshot(dataframe);
        }
    }
}