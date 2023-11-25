using System.Collections.Generic;
using System.Text;
using Common.DI;
using GameCore.Common;
using GameCore.RoundMissions.LocalMessages;
using GameCore.RoundMissions.Missions;
using LocalMessages;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;

namespace GameCore.RoundMissions
{
    public class MissionsController : MonoBehaviour
    {
        public MissionsData Data => _data;
        public RoundData RoundData { get; private set; }
        
        [SerializeField] private MissionsData _data;

        [Inject] private LocalMessageBroker _messageBroker;
        [Inject] private WindowsSystem _windowsSystem;

        private StringBuilder _stringBuilder;
        private List<MissionBase> _missions;
        
        public void Initialize()
        {
            RoundData = new RoundData();
            _stringBuilder = new StringBuilder();
            
            _missions = new List<MissionBase>
            {
                GameContainer.Create<SaveAgentsMission>(),
                GameContainer.Create<FindCactusMission>(),
                GameContainer.Create<EvacuateMission>(),
            };
            
            UpdateMissionsState();
        }
        
        public void UpdateMissionsState()
        {
            _stringBuilder.Clear();
            bool allCompleted = true;
            for (int i = 0; i < _missions.Count; i++)
            {
                var mission = _missions[i];
                mission.Update();

                if (!mission.IsCompleted)
                    allCompleted = false;
                
                _stringBuilder.Append(" ");
                _stringBuilder.Append(i + 1);
                _stringBuilder.Append(". ");

                if (mission.IsCompleted)
                    _stringBuilder.Append("<s>");

                _stringBuilder.Append(mission.MissionText);

                if (mission.IsCompleted)
                    _stringBuilder.Append("</s>");

                _stringBuilder.Append("\n");
            }

            if (allCompleted)
            {
                var message = new AllMissionsCompletedMessage();
                _messageBroker.Trigger(ref message);
            }

            if (_windowsSystem.TryGetWindow<InGameUI>(out var inGameUI))
                inGameUI.SetMissionsText(_stringBuilder.ToString());
        }

        private void OnDestroy()
        {
            foreach (var mission in _missions)
            {
                mission.Dispose();
            }
            _missions.Clear();
        }
    }
}