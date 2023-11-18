using Common;
using GameCore.RoundMissions.LocalMessages;
using LocalMessages;

namespace GameCore.RoundMissions.Missions
{
    public class SaveAgentsMission : MissionBase
    {
        public sealed override string MissionText { get; protected set; }

        private readonly LocalMessageBroker _messageBroker;
        
        public SaveAgentsMission(MissionsController controller) : base(controller)
        {
            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<AgentSavedMessage>(OnAgentSaved);
            
            MissionText = controller.Data.saveAgentsText;
        }

        public override void Dispose()
        {
            _messageBroker.Unsubscribe<AgentSavedMessage>(OnAgentSaved);
        }
        
        private void OnAgentSaved(ref AgentSavedMessage value)
        {
            if (IsCompleted) return;

            Controller.RoundData.AgentsSaved++;
            if (Controller.RoundData.AgentsSaved >= Controller.Data.agentsToSave)
                Complete();
            
            Controller.UpdateMissionsState();
        }
    }
}