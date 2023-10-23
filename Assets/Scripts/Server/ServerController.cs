using System.Collections.Generic;
using NetFrame.Server;
using SibGameJam.Common;
using SibGameJam.Common.LocalMessages;
using SibGameJam.Common.LocalMessages.MessageTypes;
using SibGameJam.Datagrams;
using UnityEngine;

namespace SibGameJam.Server
{
    public class ServerController : MonoBehaviour
    {
        private NetFrameServer _server;

        private void Start()
        {
            _server = new NetFrameServer();

            _server.ClientConnection += OnClientConnected;
            _server.ClientDisconnect += OnClientDisconnected;
            _server.Subscribe<ClientConnectInfoDatagram>(ClientInfoDatagramHandler);
            
            _server.Start(GameConst.ListenPort, GameConst.MaxClients);
        }

        private void Update()
        {
            _server.Run();
        }

        private void OnDestroy()
        {
            _server.Stop();
        }

        private void OnClientConnected(int clientId)
        {
            foreach (var player in GameData.Players)
            {
                if (player.Key == clientId) continue;

                var datagram = new PlayerJoinedDatagram
                {
                    ClientId = player.Key,
                    Info = new ClientConnectInfoDatagram
                    {
                        Name = player.Value.Name
                    },
                };
                _server.Send(ref datagram, clientId);
            }
        }
        
        private void OnClientDisconnected(int clientId)
        {
            var datagram = new PlayerLeftDatagram
            {
                Id = clientId
            };
            _server.SendAll(ref datagram);
            GameData.Players.Remove(clientId);

            var message = new UpdatePlayersListMessage();
            LocalMessageBroker.Trigger(ref message);
        }

        private void ClientInfoDatagramHandler(ClientConnectInfoDatagram datagram, int clientId)
        {
            var joinedDatagram = new PlayerJoinedDatagram
            {
                ClientId = clientId,
                Info = datagram
            };
            
            foreach (var player in GameData.Players)
            {
                _server.Send(ref joinedDatagram, player.Key);
            }
            
            GameData.Players.Add(clientId, new PlayerInfo
            {
                ClientId = clientId,
                Name = datagram.Name
            });

            var message = new UpdatePlayersListMessage();
            LocalMessageBroker.Trigger(ref message);
        }
    }
}