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

        private bool _subscribed;
        
        private void Start()
        {
            if (!_networkObject.IsOnline)
                return;
            
            _networkObject.Client.Client.Subscribe<NetworkObjectSetTickDataframe>(SetTick);
            _networkObject.Client.Client.Subscribe<NetworkObjectInterpolatePositionDataframe>(PushPositionSnapshot);
            _subscribed = true;
        }

        private void OnDestroy()
        {
            if (!_subscribed)
                return;

            _networkObject.Client.Client.Unsubscribe<NetworkObjectSetTickDataframe>(SetTick);
            _networkObject.Client.Client.Unsubscribe<NetworkObjectInterpolatePositionDataframe>(PushPositionSnapshot);
        }

        private void Update()
        {
            ref var snapshot = ref _interpolator.Current;
            transform.SetPositionAndRotation(snapshot.Position, snapshot.Rotation);
        }

        private void SetTick(NetworkObjectSetTickDataframe dataframe)
        {
            _interpolator.SetOwnerTick(dataframe.tick);
        }

        private void PushPositionSnapshot(NetworkObjectInterpolatePositionDataframe dataframe)
        {
            if (dataframe.objectId != _networkObject.Id)
                return;
            
            _interpolator.AddSnapshot(dataframe);
        }
    }
}