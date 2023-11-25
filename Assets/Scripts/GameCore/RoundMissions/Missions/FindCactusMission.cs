using Common.DI;
using GameCore.RoundMissions.LocalMessages;
using LocalMessages;
using UnityEngine;

namespace GameCore.RoundMissions.Missions
{
    public class FindCactusMission : MissionBase
    {
        public sealed override string MissionText { get; protected set; }

        private LocalMessageBroker _messageBroker;
        
        [Construct]
        public FindCactusMission(MissionsController controller, LocalMessageBroker messageBroker) : base(controller)
        {
            MissionText = controller.Data.findCactusText;
            
            _messageBroker = messageBroker;
            _messageBroker.Subscribe<CactusFoundMessage>(OnCactusFound);
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
        }
    }
}