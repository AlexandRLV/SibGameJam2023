using Common.DI;
using GameCore.Common.Messages;
using LocalMessages;

namespace GameCore.RoundMissions.Missions
{
    public class EvacuateMission : MissionBase
    {
        public sealed override string MissionText { get; protected set; }

        private LocalMessageBroker _messageBroker;
        
        [Construct]
        public EvacuateMission(MissionsController controller, LocalMessageBroker messageBroker) : base(controller)
        {
            MissionText = controller.Data.evacuationText;

            _messageBroker = messageBroker;
            _messageBroker.Subscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
        }

        public override void Update()
        {
            if (!controller.RoundData.CactusFound) return;
            if (controller.RoundData.AgentsSaved < controller.Data.agentsToSave) return;
            
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
            controller.UpdateMissionsState();
        }
    }
}