using System.Collections.Generic;
using Common.DI;
using GameCore.Enemies;
using GameCore.Enemies.EnemyObject;
using GameCore.LevelObjects.Abstract;
using GameCore.LevelObjects.InteractiveObjects;
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
        public EvacuationInteractive evacuation;
        
        private List<InteractiveObject> _interactiveObjects = new();
        private List<PushableObject> _pushableObjects = new();
        private List<EnemyController> _enemies = new();
        private List<Mousetrap> _mousetraps = new();

        private GameClientData _gameClientData;
        private LocalMessageBroker _messageBroker;
        
        [Construct]
        public LevelObjectService(GameClientData gameClientData, LocalMessageBroker messageBroker)
        {
            _messageBroker = messageBroker;
            _messageBroker.Subscribe<InteractedWithObjectDataframe>(OnInteracted);
            _messageBroker.Subscribe<PushablePositionDataframe>(OnPushableMoved);
            _messageBroker.Subscribe<EnemyDetectPlayerDataframe>(OnEnemyDetectPlayer);
            _messageBroker.Subscribe<EnemyAlertPlayerDataframe>(OnEnemyAlert);
            _messageBroker.Subscribe<ActivateMouseTrapDataframe>(OnMousetrapActivated);
        }

        public void Dispose()
        {
            _messageBroker.Unsubscribe<InteractedWithObjectDataframe>(OnInteracted);
            _messageBroker.Unsubscribe<PushablePositionDataframe>(OnPushableMoved);
            _messageBroker.Unsubscribe<EnemyDetectPlayerDataframe>(OnEnemyDetectPlayer);
            _messageBroker.Unsubscribe<EnemyAlertPlayerDataframe>(OnEnemyAlert);
            _messageBroker.Unsubscribe<ActivateMouseTrapDataframe>(OnMousetrapActivated);
        }

        public void RegisterInteractiveObject(InteractiveObject value) => _interactiveObjects.Add(value);
        public void UnregisterInteractiveObject(InteractiveObject value) => _interactiveObjects.Remove(value);

        public void RegisterPushableObject(PushableObject value) => _pushableObjects.Add(value);
        public void UnregisterPushableObject(PushableObject value) => _pushableObjects.Remove(value);

        public void RegisterEnemy(EnemyController value) => _enemies.Add(value);
        public void UnregisterEnemy(EnemyController value) => _enemies.Remove(value);

        public void RegisterMousetrap(Mousetrap value) => _mousetraps.Add(value);
        public void UnregisterMousetrap(Mousetrap value) => _mousetraps.Remove(value);

        private void OnInteracted(ref InteractedWithObjectDataframe dataframe)
        {
            if (!TryFindObject(_interactiveObjects, dataframe.objectPosition, out var target))
                return;

            // This is also because of initialization order that requires this class to be initialized before others
            // And on creation of this class, there's no registration for remote player
            var remotePlayer = GameContainer.InGame.Resolve<RemotePlayer>();
            target.InteractWithoutPlayer(remotePlayer.transform.position);
        }

        private void OnPushableMoved(ref PushablePositionDataframe dataframe)
        {
            if (!TryFindObject(_pushableObjects, dataframe.startPosition, out var target))
                return;

            target.transform.SetPositionAndRotation(dataframe.position, dataframe.rotation);
        }

        private void OnEnemyDetectPlayer(ref EnemyDetectPlayerDataframe dataframe)
        {
            if (!TryFindObject(_enemies, dataframe.checkPosition, out var enemy))
                return;
            
            if (dataframe.isDetect)
            {
                var otherMouseType = _gameClientData.IsMaster ? PlayerMouseType.ThinMouse : PlayerMouseType.FatMouse;
                enemy.DetectPlayer(otherMouseType, false);
            }
            else
            {
                enemy.UndetectPlayer();
            }
        }

        private void OnEnemyAlert(ref EnemyAlertPlayerDataframe dataframe)
        {
            var message = new PlayerDetectedMessage
            {
                PlayerPosition = dataframe.playerPosition
            };
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
            GameContainer.Common.Resolve<SoundService>().PlaySound(SoundType.Alert);
        }
        
        private void OnMousetrapActivated(ref ActivateMouseTrapDataframe dataframe)
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