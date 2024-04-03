using Common;
using Common.DI;
using GameCore.Levels;
using Localization;
using Localization.Extensions;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        public bool Active
        {
            set => gameObject.SetActive(value);
        }

        [SerializeField] private TextMeshProUGUI _levelNameText;
        [SerializeField] private TextMeshProUGUI _hintText;

        [Inject] private LocalizationProvider _localizationProvider;

        private void Awake()
        {
            GameContainer.InjectToInstance(this);
        }

        public void SetLevel(LevelInfo levelInfo)
        {
            _levelNameText.text = $"\"{_localizationProvider.GetTextLocalization(levelInfo.levelNameKey)}\"";
            _hintText.text = _localizationProvider.GetTextLocalization(levelInfo.hintsKeys.GetRandom());
        }
    }
}