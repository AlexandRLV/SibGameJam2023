using Common;
using GameCore.RoundMissions.LocalMessages;
using LocalMessages;

namespace GameCore.RoundMissions.Missions
{
    public class FindCactusMission : MissionBase
    {
        public sealed override string MissionText { get; protected set; }

        private readonly LocalMessageBroker _messageBroker;
        
        public FindCactusMission(MissionsController controller) : base(controller)
        {
            MissionText = controller.Data.findCactusText;
            
            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<CactusFoundMessage>(OnCactusFound);
        }

        public override void Dispose()
        {
            _messageBroker.Unsubscribe<CactusFoundMessage>(OnCactusFound);
        }

        private void OnCactusFound(ref CactusFoundMessage value)
        {
            Controller.RoundData.CactusFound = true;
            Complete();
            Controller.UpdateMissionsState();
        }
    }
}