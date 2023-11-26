using System.Collections.Generic;
using Common.DI;
using Networking;
using Networking.Dataframes.InGame.LevelObjects;
using UnityEngine;

namespace GameCore.NetworkObjects
{
    public class NetworkObjectsController : MonoBehaviour
    {
        [Inject] private GameClient _gameClient;
        [Inject] private RemoteNetworkObjects _remoteNetworkObjects;

        private int _objectId;
        private bool _subscribed;

        private List<NetworkObject> _spawnedObjects;

        private void Start()
        {
            _spawnedObjects = new List<NetworkObject>(); 
            if (!_gameClient.IsConnected) return;
            if (_gameClient.IsMaster)
                return;
            
            _gameClient.Client.Subscribe<CreateNetworkObjectDataframe>(CreateNetworkObject);
            _subscribed = true;
        }

        private void OnDestroy()
        {
            if (_subscribed)
                _gameClient.Client.Unsubscribe<CreateNetworkObjectDataframe>(CreateNetworkObject);

            foreach (var spawnedObject in _spawnedObjects)
            {
                Destroy(spawnedObject.gameObject);
            }
            _spawnedObjects.Clear();
        }

        public void RegisterObject(NetworkObject target)
        {
            if (!_gameClient.IsConnected) return;
            if (!_gameClient.IsMaster)
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
        
        private void CreateNetworkObject(CreateNetworkObjectDataframe dataframe)
        {
            var prefab = _remoteNetworkObjects.GetPrefabByType(dataframe.objectType);
            var networkObject = Object.Instantiate(prefab, dataframe.position, dataframe.rotation);
            _spawnedObjects.Add(networkObject);
        }
    }
}