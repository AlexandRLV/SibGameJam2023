using GameCore.Player.Network;
using Networking.Dataframes.InGame.LevelObjects;
using UnityEngine;

namespace GameCore.NetworkObjects
{
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class NetworkObjectPositionSender : MonoBehaviour
    {
        [SerializeField] private NetworkObject _networkObject;
        [SerializeField] private NetworkParameters _networkParameters;

        private int _tick;

        private void FixedUpdate()
        {
            if (!_networkObject.IsOnline)
                return;
            
            _tick++;
            if (_tick % _networkParameters.SendTickUpdateRate == 0)
                SendCurrentTick();

            if (_tick % _networkParameters.SendPositionUpdateRate == 0)
                SendPosition();
        }

        private void SendCurrentTick()
        {
            var dataframe = new NetworkObjectSetTickDataframe
            {
                objectId = _networkObject.Id,
                tick = _tick
            };
            _networkObject.Client.Send(ref dataframe);
        }

        private void SendPosition()
        {
            var dataframe = new NetworkObjectInterpolatePositionDataframe
            {
                objectId = _networkObject.Id,
                Tick = _tick,
                Teleported = _networkObject.Teleported,
                Position = transform.position,
                Rotation = transform.rotation,
            };
            _networkObject.Client.Send(ref dataframe);

            _networkObject.Teleported = false;
        }
    }
}