using TMPro;
using UnityEngine;

namespace UI.Tween.Tweens
{
    public class TweenTextTyping : TweenBase
    {
        [SerializeField] private TextMeshProUGUI _targetText;
        [SerializeField] private float _timeForOneSymbol;

        private string _initialText;

        public override void ReadCurrentToInitial()
        {
            _initialText = _targetText.text;
            duration = _initialText.Length * _timeForOneSymbol;
        }

        public override void SetCurrentToInitial()
        {
            _targetText.text = _initialText;
            _targetText.gameObject.SetActive(false);
        }

        protected override void OnUpdateInternal(float t)
        {
            _targetText.gameObject.SetActive(true);
            if (Mathf.Approximately(t, 1f))
            {
                _targetText.text = _initialText;
                return;
            }
            
            int length = Mathf.CeilToInt(Mathf.Lerp(0, _initialText.Length, t));
            length = Mathf.Clamp(length, 0, _initialText.Length - 1);
            _targetText.text = _initialText.Remove(length);
        }
    }
}