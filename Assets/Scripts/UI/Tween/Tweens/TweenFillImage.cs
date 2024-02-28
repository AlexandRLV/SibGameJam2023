using UnityEngine;
using UnityEngine.UI;

namespace UI.Tween.Tweens
{
    public class TweenFillImage : TweenBase
    {
        [SerializeField] private float _from;
        [SerializeField] private float _to;
        [SerializeField] private Image _target;

        private float _initial;

        public override void ReadCurrentToInitial()
        {
            _initial = _target.fillAmount;
        }

        public override void SetCurrentToInitial()
        {
            _target.fillAmount = _initial;
        }

        protected override void OnUpdateInternal(float t)
        {
            _target.fillAmount = Mathf.Lerp(_from, _to, t);
        }
    }
}