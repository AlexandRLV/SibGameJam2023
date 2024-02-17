using Common.DI;
using GameCore.Common.Messages;
using GameCore.LevelObjects.Messages;
using LocalMessages;
using UnityEngine;

namespace GameCore.RoundMissions.Missions
{
    public class EvacuateMission : MissionBase
    {
        public sealed override string MissionText { get; protected set; }

        private readonly LocalMessageBroker _messageBroker;
        
        [Construct]
        public EvacuateMission(MissionsController controller, LocalMessageBroker messageBroker) : base(controller)
        {
            MissionText = controller.Data.evacuationText;

            _messageBroker = messageBroker;
            _messageBroker.Subscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
        }

        public override void Update()
        {
            foreach (var mission in controller.activeMissions)
            {
                if (mission == this) continue;
                if (!mission.IsCompleted) return;
            }
            
            Debug.Log("Activating evacuation, all missions completed");
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
            Debug.Log("Completing evacuation mission");
            Complete();
            controller.UpdateMissionsState();
        }
    }
}