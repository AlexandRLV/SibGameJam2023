﻿using System.Collections.Generic;
using Common.DI;
using GameCore.Enemies;
using GameCore.Enemies.EnemyObject;
using GameCore.LevelObjects.Abstract;
using GameCore.LevelObjects.TriggerObjects;
using GameCore.Player;
using GameCore.Player.Network;
using GameCore.Sounds;
using LocalMessages;
using Networking;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.LevelObjects
{
    public class LevelObjectService
    {
        private List<InteractiveObject> _interactiveObjects = new();
        private List<PushableObject> _pushableObjects = new();
        private List<EnemyController> _enemies = new();
        private List<Mousetrap> _mousetraps = new();

        private GameClient _gameClient;
        
        [Construct]
        public LevelObjectService(GameClient gameClient)
        {
            _gameClient = gameClient;
            _gameClient.Client.Subscribe<InteractedWithObjectDataframe>(OnInteracted);
            _gameClient.Client.Subscribe<PushablePositionDataframe>(OnPushableMoved);
            _gameClient.Client.Subscribe<EnemyDetectPlayerDataframe>(OnEnemyDetectPlayer);
            _gameClient.Client.Subscribe<EnemyAlertPlayerDataframe>(OnEnemyAlert);
            _gameClient.Client.Subscribe<ActivateMouseTrapDataframe>(OnMousetrapActivated);
        }

        public void Dispose()
        {
            _gameClient.Client.Unsubscribe<InteractedWithObjectDataframe>(OnInteracted);
            _gameClient.Client.Unsubscribe<PushablePositionDataframe>(OnPushableMoved);
            _gameClient.Client.Unsubscribe<EnemyDetectPlayerDataframe>(OnEnemyDetectPlayer);
            _gameClient.Client.Unsubscribe<EnemyAlertPlayerDataframe>(OnEnemyAlert);
            _gameClient.Client.Unsubscribe<ActivateMouseTrapDataframe>(OnMousetrapActivated);
        }

        public void RegisterInteractiveObject(InteractiveObject value) => _interactiveObjects.Add(value);
        public void UnregisterInteractiveObject(InteractiveObject value) => _interactiveObjects.Remove(value);

        public void RegisterPushableObject(PushableObject value) => _pushableObjects.Add(value);
        public void UnregisterPushableObject(PushableObject value) => _pushableObjects.Remove(value);

        public void RegisterEnemy(EnemyController value) => _enemies.Add(value);
        public void UnregisterEnemy(EnemyController value) => _enemies.Remove(value);

        public void RegisterMousetrap(Mousetrap value) => _mousetraps.Add(value);
        public void UnregisterMousetrap(Mousetrap value) => _mousetraps.Remove(value);

        private void OnInteracted(InteractedWithObjectDataframe dataframe)
        {
            if (!TryFindObject(_interactiveObjects, dataframe.objectPosition, out var target))
                return;

            // This is also because of initialization order that requires this class to be initialized before others
            // And on creation of this class, there's no registration for remote player
            var remotePlayer = GameContainer.InGame.Resolve<RemotePlayer>();
            target.InteractWithoutPlayer(remotePlayer.transform.position);
        }

        private void OnPushableMoved(PushablePositionDataframe dataframe)
        {
            if (!TryFindObject(_pushableObjects, dataframe.startPosition, out var target))
                return;

            target.transform.SetPositionAndRotation(dataframe.position, dataframe.rotation);
        }

        private void OnEnemyDetectPlayer(EnemyDetectPlayerDataframe dataframe)
        {
            if (!TryFindObject(_enemies, dataframe.checkPosition, out var enemy))
                return;
            
            if (dataframe.isDetect)
            {
                var otherMouseType = _gameClient.IsMaster ? PlayerMouseType.ThinMouse : PlayerMouseType.FatMouse;
                enemy.DetectPlayer(otherMouseType, false);
            }
            else
            {
                enemy.UndetectPlayer();
            }
        }

        private void OnEnemyAlert(EnemyAlertPlayerDataframe dataframe)
        {
            var message = new PlayerDetectedMessage
            {
                PlayerPosition = dataframe.playerPosition
            };
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
            GameContainer.Common.Resolve<SoundService>().PlaySound(SoundType.Alert);
        }
        
        private void OnMousetrapActivated(ActivateMouseTrapDataframe dataframe)
        {
            if (!TryFindObject(_mousetraps, dataframe.mousetrapPosition, out var mousetrap))
                return;
            
            mousetrap.Activate();
        }

        private static bool TryFindObject<T>(List<T> objects, Vector3 startPosition, out T target) where T : ICheckPositionObject
        {
            target = default;
            float minDistance = float.MaxValue;
            foreach (var checkPositionObject in objects)
            {
                float distance = Vector3.Distance(startPosition, checkPositionObject.CheckPosition);
                if (distance > minDistance) continue;

                target = checkPositionObject;
                minDistance = distance;
            }

            return target != null;
        }
    }
}