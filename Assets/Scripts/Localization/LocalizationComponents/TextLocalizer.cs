using System.Collections.Generic;
using Common.DI;
using Localization.Extensions;
using LocalMessages;
using LocalMessages.MessageTypes;
using TMPro;
using UnityEngine;

namespace Localization.LocalizationComponents
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

        public void SetKey(string key)
        {
            _key = key;
            UpdateLocalization();
        }

        private void OnLocalizationChanged(ref LocalizationChangedMessage message)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            if (string.IsNullOrWhiteSpace(_key))
                _text.text = "";
            else if (_parameters == null || _parameters.Count == 0)
                _text.text = _localizationProvider.GetTextLocalization(_key);
            else
                _text.text = _localizationProvider.GetTextLocalization(_key, _parameters);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_text == null)
                _text = GetComponent<TextMeshProUGUI>();
        }
#endif
    }
}