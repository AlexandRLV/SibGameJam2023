using System.Collections.Generic;
using System.Text;
using Common.DI;
using GameCore.Common;
using GameCore.RoundMissions.LocalMessages;
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
        
        public List<MissionBase> activeMissions;

        [Inject] private MissionsData _data;
        [Inject] private LocalMessageBroker _messageBroker;
        [Inject] private WindowsSystem _windowsSystem;

        private StringBuilder _stringBuilder;
        
        public void Initialize(List<MissionBase> missions)
        {
            activeMissions = missions;
            
            RoundData = new RoundData();
            _stringBuilder = new StringBuilder();
            
            UpdateMissionsState();
        }
        
        public void UpdateMissionsState()
        {
            _stringBuilder.Clear();
            bool allCompleted = true;
            for (int i = 0; i < activeMissions.Count; i++)
            {
                var mission = activeMissions[i];
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
            foreach (var mission in activeMissions)
            {
                mission.Dispose();
            }
            activeMissions.Clear();
        }
    }
}