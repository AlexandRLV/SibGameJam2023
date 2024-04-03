using System.Collections.Generic;
using System.Text;
using Common.DI;
using GameCore.RoundControl;
using GameCore.RoundMissions.LocalMessages;
using Localization;
using Localization.Extensions;
using LocalMessages;
using LocalMessages.MessageTypes;
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
        [Inject] private LocalizationProvider _localizationProvider;

        private StringBuilder _stringBuilder;
        
        public void Initialize(List<MissionBase> missions)
        {
            activeMissions = missions;
            
            RoundData = new RoundData();
            _stringBuilder = new StringBuilder();
            
            UpdateMissionsState();
            
            _messageBroker.Subscribe<LocalizationChangedMessage>(OnLocalizationMessage);
        }

        public void UpdateMissionsState()
        {
            UpdateMissionsText();
            
            foreach (var mission in activeMissions)
            {
                if (!mission.IsCompleted)
                    return;
            }
            
            var message = new AllMissionsCompletedMessage();
            _messageBroker.Trigger(ref message);
        }

        private void UpdateMissionsText()
        {
            _stringBuilder.Clear();
            for (int i = 0; i < activeMissions.Count; i++)
            {
                var mission = activeMissions[i];
                mission.Update();
                
                _stringBuilder.Append(" ");
                _stringBuilder.Append(i + 1);
                _stringBuilder.Append(". ");

                if (mission.IsCompleted)
                    _stringBuilder.Append("<s>");

                string localizedText = _localizationProvider.GetTextLocalization(mission.MissionLocalizationKey);
                _stringBuilder.Append(localizedText);

                if (mission.IsCompleted)
                    _stringBuilder.Append("</s>");

                _stringBuilder.Append("\n");
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
            
            _messageBroker.Unsubscribe<LocalizationChangedMessage>(OnLocalizationMessage);
        }

        private void OnLocalizationMessage(ref LocalizationChangedMessage message)
        {
            UpdateMissionsText();
        }
    }
}