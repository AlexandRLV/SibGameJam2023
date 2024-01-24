using Common.DI;
using TMPro;
using UnityEngine;

namespace Localization
{
    public class TextLocalizer : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private TextMeshProUGUI _text;

        [Inject] private LocalizationProvider _localizationProvider;

        private void Awake()
        {
            GameContainer.InjectToInstance(this);
            _text.text = _localizationProvider.GetLocalization(_key);
        }
    }
}