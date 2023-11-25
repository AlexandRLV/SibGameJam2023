using Common.DI;
using GameCore.Common.Messages;
using GameCore.LevelObjects.Abstract;
using GameCore.LevelObjects.Messages;
using LocalMessages;

namespace GameCore.LevelObjects.InteractiveObjects
{
    public class EvacuationInteractive : BaseTriggerObject
    {
        [Inject] private LocalMessageBroker _messageBroker;
        
        private void Start()
        {
            _messageBroker.Subscribe<ActivateEvacuationMessage>(OnEvacuationActivated);
            gameObject.SetActive(false);
        }

        private void OnEvacuationActivated(ref ActivateEvacuationMessage value)
        {
            gameObject.SetActive(value.active);
        }

        protected override void OnPlayerEnter()
        {
            var message = new PlayerEvacuatedMessage();
            _messageBroker.Trigger(ref message);
        }

        protected override void OnPlayerStay()
        {
        }

        protected override void OnPlayerExit()
        {
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<ActivateEvacuationMessage>(OnEvacuationActivated);
        }
    }
}