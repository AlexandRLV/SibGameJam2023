using Common;
using NetFrame.Client;
using NetFrame.Enums;
using Startup;
using UnityEngine;

namespace Networking
{
    public class GameClient : MonoBehaviour
    {
        public bool IsConnected { get; private set; }
        
        private NetFrameClient _client;
        private ClientParameters _parameters;

        public void Initialize(ClientParameters parameters)
        {
            _parameters = parameters;
            _client = GameContainer.Common.Resolve<NetFrameClient>();

            _client.ConnectionSuccessful += OnConnectionSuccessfull;
            _client.ConnectedFailed += OnConnectionFailed;
            _client.Disconnected += OnDisconnected;
            
            _client.Connect(_parameters.Ip, _parameters.port);
        }

        public void Shutdown()
        {
            _client.ConnectionSuccessful -= OnConnectionSuccessfull;
            _client.ConnectedFailed -= OnConnectionFailed;
            _client.Disconnected -= OnDisconnected;
            _client.Disconnect();
        }

        private void OnConnectionSuccessfull()
        {
            IsConnected = true;
        }

        private void OnConnectionFailed(ReasonServerConnectionFailed reason)
        {
            IsConnected = false;
        }

        private void OnDisconnected()
        {
            IsConnected = false;
        }

        private void OnDestroy() => Shutdown();
    }
}