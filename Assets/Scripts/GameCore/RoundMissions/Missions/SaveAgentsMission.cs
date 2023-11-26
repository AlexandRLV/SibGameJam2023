using Common.DI;
using GameCore.RoundMissions.LocalMessages;
using LocalMessages;
using UI.NotificationsSystem;

namespace GameCore.RoundMissions.Missions
{
    public class SaveAgentsMission : MissionBase
    {
        public sealed override string MissionText { get; protected set; }

        private readonly LocalMessageBroker _messageBroker;
        private readonly NotificationsManager _notificationsManager;
        
        [Construct]
        public SaveAgentsMission(MissionsController controller, LocalMessageBroker messageBroker, NotificationsManager notificationsManager) : base(controller)
        {
            MissionText = controller.Data.saveAgentsText;
            
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
            
            int remainingAgents = controller.Data.agentsToSave - controller.RoundData.AgentsSaved;
            string text = remainingAgents switch
            {
                2 => "Агент спасён! Осталось двое!",
                1 => "Агент спасён! Остался всего один!",
                0 => "Все агенты спасены!",
                _ => "Агент спасён! Так держать!"
            };
            _notificationsManager.ShowNotification(text, NotificationsManager.NotificationType.Top);
            
            if (controller.RoundData.AgentsSaved >= controller.Data.agentsToSave)
                Complete();
            
            controller.UpdateMissionsState();
        }
    }
}