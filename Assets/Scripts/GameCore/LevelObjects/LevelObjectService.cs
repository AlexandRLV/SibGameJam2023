using System.Collections.Generic;
using Common;
using GameCore.InteractiveObjects;
using NetFrame.Client;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.LevelObjects
{
    public class LevelObjectService
    {
        private List<InteractiveObject> _objects = new();
        private List<PushableObject> _pushableObjects = new();

        private NetFrameClient _client;
        
        public LevelObjectService()
        {
            _client = GameContainer.Common.Resolve<NetFrameClient>();
            
            _client.Subscribe<InteractedWithObjectDataframe>(OnInteracted);
            _client.Subscribe<PushablePositionDataframe>(OnPushableMoved);
        }

        public void Dispose()
        {
            _client.Unsubscribe<InteractedWithObjectDataframe>(OnInteracted);
            _client.Unsubscribe<PushablePositionDataframe>(OnPushableMoved);
        }

        public void RegisterInteractiveObject(InteractiveObject value) => _objects.Add(value);
        public void UnregisterInteractiveObject(InteractiveObject value) => _objects.Remove(value);

        public void RegisterPushableObject(PushableObject value) => _pushableObjects.Add(value);
        public void UnregisterPushableObject(PushableObject value) => _pushableObjects.Remove(value);

        private void OnInteracted(InteractedWithObjectDataframe dataframe)
        {
            InteractiveObject target = null;
            float minDistance = float.MaxValue;
            foreach (var interactiveObject in _objects)
            {
                if (interactiveObject.Type != dataframe.interactedObject)
                    continue;

                float distance = Vector3.Distance(dataframe.objectPosition, interactiveObject.transform.position);
                if (distance > minDistance) continue;

                target = interactiveObject;
                minDistance = distance;
            }
            
            if (target == null) return;
            
            target.InteractWithoutPlayer();
        }

        private void OnPushableMoved(PushablePositionDataframe dataframe)
        {
            PushableObject target = null;
            float minDistance = float.MaxValue;
            foreach (var pushableObject in _pushableObjects)
            {
                float distance = Vector3.Distance(dataframe.startPosition, pushableObject.StartPosition);
                if (distance > minDistance) continue;

                target = pushableObject;
                minDistance = distance;
            }
            
            if (target == null) return;

            target.transform.SetPositionAndRotation(dataframe.position, dataframe.rotation);
        }
    }
}