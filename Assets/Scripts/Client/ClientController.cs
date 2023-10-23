using NetFrame.Client;
using NetFrame.Enums;
using SibGameJam.Common;
using SibGameJam.Common.LocalMessages;
using SibGameJam.Common.LocalMessages.MessageTypes;
using SibGameJam.Datagrams;
using UnityEngine;

namespace SibGameJam.Client
{
    public class ClientController : MonoBehaviour
    {
        private NetFrameClient _client;

        private void Start()
        {
            _client = new NetFrameClient();
            
            _client.ConnectionSuccessful += OnConnectionSuccessfull;
            _client.ConnectedFailed += OnConnectionFailed;
            _client.Disconnected += OnDisconnected;
            
            _client.Subscribe<PlayerJoinedDatagram>(OnPlayerJoined);
            _client.Subscribe<PlayerLeftDatagram>(OnPlayerLeft);
            
            _client.Connect(GameConst.ConnectToIp, GameConst.ListenPort);
        }

        private void Update()
        {
            _client.Run();
        }

        private void OnDestroy()
        {
            _client.Disconnect();
        }

        private void OnConnectionSuccessfull()
        {
            var infoDatagram = new ClientConnectInfoDatagram
            {
                Name = ClientData.ClientName
            };
            _client.Send(ref infoDatagram);
        }

        private void OnConnectionFailed(ReasonServerConnectionFailed reason)
        {
            Debug.LogError($"Connection failed: {reason}");
        }

        private void OnDisconnected()
        {
            
        }

        private void OnPlayerJoined(PlayerJoinedDatagram datagram)
        {
            GameData.Players.Add(datagram.ClientId, new PlayerInfo
            {
                ClientId = datagram.ClientId,
                Name = datagram.Info.Name
            });

            var message = new UpdatePlayersListMessage();
            LocalMessageBroker.Trigger(ref message);
        }

        private void OnPlayerLeft(PlayerLeftDatagram datagram)
        {
            GameData.Players.Remove(datagram.Id);

            var message = new UpdatePlayersListMessage();
            LocalMessageBroker.Trigger(ref message);
        }
    }
}