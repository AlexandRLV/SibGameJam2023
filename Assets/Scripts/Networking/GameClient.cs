using Common;
using LocalMessages;
using NetFrame.Client;
using NetFrame.Enums;
using Networking.LocalMessages;
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
        }

        public void Connect()
        {
            _client.Connect(_parameters.Ip, _parameters.port);
        }

        public void Disconnect()
        {
            _client.Disconnect();
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
            var message = new ConnectedMessage();
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        }

        private void OnConnectionFailed(ReasonServerConnectionFailed reason)
        {
            if (reason == ReasonServerConnectionFailed.AlreadyConnected)
                return;
            
            IsConnected = false;
            var message = new ConnectionFailedMessage
            {
                reason = reason
            };
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        }

        private void OnDisconnected()
        {
            IsConnected = false;
            var message = new DisconnectedMessage();
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        }

        private void OnDestroy() => Shutdown();
    }
}