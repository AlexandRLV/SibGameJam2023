using Common.DI;
using GameCore.Common.Messages;
using GameCore.LevelObjects.Messages;
using LocalMessages;

namespace GameCore.RoundMissions.Missions
{
    public class EvacuateMission : MissionBase
    {
        public sealed override string MissionLocalizationKey { get; protected set; }

        private readonly LocalMessageBroker _messageBroker;
        
        [Construct]
        public EvacuateMission(MissionsController controller, LocalMessageBroker messageBroker) : base(controller)
        {
            MissionLocalizationKey = controller.Data.evacuationLocalizationKey;

            _messageBroker = messageBroker;
            _messageBroker.Subscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
        }

        public override void Update()
        {
            if (IsCompleted) return;
            
            foreach (var mission in controller.activeMissions)
            {
                if (mission == this) continue;
                if (!mission.IsCompleted) return;
            }
            
            var message = new ActivateEvacuationMessage
            {
                active = true
            };
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