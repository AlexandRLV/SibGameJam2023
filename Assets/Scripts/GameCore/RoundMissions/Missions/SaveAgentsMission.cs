using Common.DI;
using GameCore.RoundMissions.LocalMessages;
using Localization;
using LocalMessages;
using UI.NotificationsSystem;

namespace GameCore.RoundMissions.Missions
{
    public class SaveAgentsMission : MissionBase
    {
        public sealed override string MissionLocalizationKey { get; protected set; }

        [Inject] private LocalizationProvider _localizationProvider;

        private readonly LocalMessageBroker _messageBroker;
        private readonly NotificationsManager _notificationsManager;
        
        [Construct]
        public SaveAgentsMission(MissionsController controller, LocalMessageBroker messageBroker, NotificationsManager notificationsManager) : base(controller)
        {
            MissionLocalizationKey = controller.Data.saveAgentsLocalizationKey;
            
            _messageBroker = messageBroker;
            _messageBroker.Subscribe<AgentSavedMessage>(OnAgentSaved);

            _notificationsManager = notificationsManager;
        }

        public override void Dispose()
        {
            _messageBroker.Unsubscribe<AgentSavedMessage>(OnAgentSaved);
        }
        
        private void OnAgentSaved(ref AgentSavedMessage value)
        {
            if (IsCompleted) return;

            controller.RoundData.AgentsSaved++;
            _notificationsManager.ShowNotification("$MISSION_COMPLETED_AGENT_SAVED", NotificationType.Top);
            
            if (controller.RoundData.AgentsSaved >= controller.Data.agentsToSave)
                Complete();
            
            controller.UpdateMissionsState();
        }
    }
}