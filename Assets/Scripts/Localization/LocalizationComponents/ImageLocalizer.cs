using Common.DI;
using Localization.Extensions;
using LocalMessages;
using LocalMessages.MessageTypes;
using UnityEngine;
using UnityEngine.UI;

namespace Localization.LocalizationComponents
{
    public class ImageLocalizer : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private Image _image;
        
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

        private void UpdateLocalization()
        {
            if (string.IsNullOrWhiteSpace(_key))
                return;

            if (_localizationProvider.TryGetImageLocalization(_key, out var sprite))
                _image.sprite = sprite;
        }

        private void OnLocalizationChanged(ref LocalizationChangedMessage message)
        {
            UpdateLocalization();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_image == null)
                _image = GetComponent<Image>();
        }
#endif
    }
}