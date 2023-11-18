using Common;
using GameCore.Common.Messages;
using LocalMessages;

namespace GameCore.RoundMissions.Missions
{
    public class EvacuateMission : MissionBase
    {
        public override string MissionText { get; protected set; }

        private LocalMessageBroker _messageBroker;
        
        public EvacuateMission(MissionsController controller) : base(controller)
        {
            MissionText = controller.Data.evacuationText;

            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
        }

        public override void Update()
        {
            if (!Controller.RoundData.CactusFound) return;
            if (Controller.RoundData.AgentsSaved < Controller.Data.agentsToSave) return;
            
            var message = new ActivateEvacuationMessage();
            message.active = true;
            _messageBroker.Trigger(ref message);
        }

        public override void Dispose()
        {
            _messageBroker.Unsubscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
        }

        private void OnPlayerEvacuated(ref PlayerEvacuatedMessage message)
        {
            Complete();
            Controller.UpdateMissionsState();
        }
    }
}