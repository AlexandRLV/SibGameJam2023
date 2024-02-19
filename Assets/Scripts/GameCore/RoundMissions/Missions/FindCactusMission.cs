using Common.DI;
using GameCore.RoundMissions.LocalMessages;
using LocalMessages;
using UI.NotificationsSystem;
using UnityEngine;

namespace GameCore.RoundMissions.Missions
{
    public class FindCactusMission : MissionBase
    {
        public sealed override string MissionText { get; protected set; }

        private readonly LocalMessageBroker _messageBroker;
        private readonly NotificationsManager _notificationsManager;
        
        [Construct]
        public FindCactusMission(MissionsController controller, LocalMessageBroker messageBroker, NotificationsManager notificationsManager) : base(controller)
        {
            MissionText = controller.Data.findCactusText;
            
            _messageBroker = messageBroker;
            _messageBroker.Subscribe<CactusFoundMessage>(OnCactusFound);
            
            _notificationsManager = notificationsManager;
        }

        public override void Dispose()
        {
            _messageBroker.Unsubscribe<CactusFoundMessage>(OnCactusFound);
        }

        private void OnCactusFound(ref CactusFoundMessage value)
        {
            controller.RoundData.CactusFound = true;
            Complete();
            controller.UpdateMissionsState();
            
            _notificationsManager.ShowNotification("Кактус найден!", NotificationType.Top);
        }
    }
}