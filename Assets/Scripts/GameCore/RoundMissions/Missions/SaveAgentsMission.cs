using Common.DI;
using GameCore.RoundMissions.LocalMessages;
using LocalMessages;

namespace GameCore.RoundMissions.Missions
{
    public class SaveAgentsMission : MissionBase
    {
        public sealed override string MissionText { get; protected set; }

        private readonly LocalMessageBroker _messageBroker;
        
        [Construct]
        public SaveAgentsMission(MissionsController controller, LocalMessageBroker messageBroker) : base(controller)
        {
            MissionText = controller.Data.saveAgentsText;
            
            _messageBroker = messageBroker;
            _messageBroker.Subscribe<AgentSavedMessage>(OnAgentSaved);
        }

        public override void Dispose()
        {
            _messageBroker.Unsubscribe<AgentSavedMessage>(OnAgentSaved);
        }
        
        private void OnAgentSaved(ref AgentSavedMessage value)
        {
            if (IsCompleted) return;

            controller.RoundData.AgentsSaved++;
            if (controller.RoundData.AgentsSaved >= controller.Data.agentsToSave)
                Complete();
            
            controller.UpdateMissionsState();
        }
    }
}