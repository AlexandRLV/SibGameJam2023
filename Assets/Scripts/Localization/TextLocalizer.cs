using System.Collections.Generic;
using Common.DI;
using LocalMessages;
using LocalMessages.MessageTypes;
using TMPro;
using UnityEngine;

namespace Localization
{
    public class TextLocalizer : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private List<LocalizationParameter> _parameters;

        [Inject] private LocalizationProvider _localizationProvider;
        [Inject] private LocalMessageBroker _messageBroker;

        private void Awake()
        {
            GameContainer.InjectToInstance(this);
            
            UpdateLocalization();
            _messageBroker.Subscribe<LocalizationChangedMessage>(OnLocalizationChanged);
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<LocalizationChangedMessage>(OnLocalizationChanged);
        }

        private void OnLocalizationChanged(ref LocalizationChangedMessage message)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            if (_parameters == null || _parameters.Count == 0)
                _text.text = _localizationProvider.GetLocalization(_key);
            else
                _text.text = _localizationProvider.GetLocalization(_key, _parameters);
        }
    }
}