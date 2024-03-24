using System.Collections.Generic;
using Common;
using Common.DI;
using GameCore.Common.Messages;
using GameCore.LevelObjects.Abstract;
using GameCore.LevelObjects.Messages;
using GameCore.Player;
using LocalMessages;
using UI.NotificationsSystem;

namespace GameCore.LevelObjects.InteractiveObjects
{
    public class EvacuationInteractive : BaseTriggerObject
    {
        [Inject] private LocalMessageBroker _messageBroker;
        [Inject] private LevelObjectService _levelObjectService;
        [Inject] private NotificationsManager _notificationsManager;

        private Dictionary<int, bool> _enteredMovementObjects;
        
        private void Start()
        {
            GameContainer.InjectToInstance(this);

            _enteredMovementObjects = new Dictionary<int, bool>();
            
            _messageBroker.Subscribe<ActivateEvacuationMessage>(OnEvacuationActivated);
            _levelObjectService.evacuation = this;
            
            gameObject.SetActive(false);
        }

        private void OnEvacuationActivated(ref ActivateEvacuationMessage value)
        {
            IsUsed = false;
            gameObject.SetActive(value.active);
            _notificationsManager.ShowNotification(Const.Notifications.EvacuationActivated); //$MISSION_COMPLETED_EVACUATION_ACTIVATED
        }

        protected override void OnPlayerEnter()
        {
            _enteredMovementObjects[Movement.gameObject.GetInstanceID()] = true;
            if (!AllMovementObjectsEntered()) return;
            
            var message = new PlayerEvacuatedMessage();
            _messageBroker.Trigger(ref message);
        }

        protected override void OnPlayerStay()
        {
        }

        protected override void OnPlayerExit()
        {
            _enteredMovementObjects[Movement.gameObject.GetInstanceID()] = false;
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<ActivateEvacuationMessage>(OnEvacuationActivated);
        }

        private bool AllMovementObjectsEntered()
        {
            var player = GameContainer.InGame.Resolve<IPlayer>();
            foreach (int movementObject in player.MovementObjects)
            {
                if (!_enteredMovementObjects.ContainsKey(movementObject) || !_enteredMovementObjects[movementObject])
                    return false;
            }

            return true;
        }
    }
}