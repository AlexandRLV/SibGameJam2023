using System.Collections.Generic;
using Common.DI;
using LocalMessages;
using Networking;
using Networking.Client;
using Networking.Dataframes.InGame.LevelObjects;
using UnityEngine;

namespace GameCore.NetworkObjects
{
    public class NetworkObjectsController : MonoBehaviour
    {
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        [Inject] private RemoteNetworkObjects _remoteNetworkObjects;
        [Inject] private LocalMessageBroker _messageBroker;

        private int _objectId;
        private bool _subscribed;

        private List<NetworkObject> _spawnedObjects;

        private void Start()
        {
            _spawnedObjects = new List<NetworkObject>(); 
            if (!_gameClientData.IsConnected) return;
            if (_gameClientData.IsMaster)
                return;
            
            _messageBroker.Subscribe<CreateNetworkObjectDataframe>(CreateNetworkObject);
            _subscribed = true;
        }

        private void OnDestroy()
        {
            if (_subscribed)
                _messageBroker.Unsubscribe<CreateNetworkObjectDataframe>(CreateNetworkObject);

            foreach (var spawnedObject in _spawnedObjects)
            {
                Destroy(spawnedObject.gameObject);
            }
            _spawnedObjects.Clear();
        }

        public void RegisterObject(NetworkObject target)
        {
            if (!_gameClientData.IsConnected) return;
            if (!_gameClientData.IsMaster)
            {
                Destroy(target.gameObject);
                return;
            }

            target.Id = _objectId++;
            target.IsOnline = true;

            var dataframe = new CreateNetworkObjectDataframe
            {
                objectId = target.Id,
                objectType = target.Type,
                position = target.transform.position,
                rotation = target.transform.rotation,
            };
            _gameClient.Send(ref dataframe);
        }
        
        private void CreateNetworkObject(ref CreateNetworkObjectDataframe dataframe)
        {
            var prefab = _remoteNetworkObjects.GetPrefabByType(dataframe.objectType);
            var networkObject = Object.Instantiate(prefab, dataframe.position, dataframe.rotation);
            _spawnedObjects.Add(networkObject);
        }
    }
}