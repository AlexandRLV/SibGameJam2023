using Common;
using GameCore.Levels;
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

        public void SetLevel(LevelInfo levelInfo)
        {
            _levelNameText.text = $"\"{levelInfo.levelName}\"";
            _hintText.text = levelInfo.hints.GetRandom();
        }
    }
}