using UnityEngine;
using UnityEngine.UI;

namespace UI.Tween.Tweens
{
    public class TweenColorImage : TweenBase
    {
        [SerializeField] private Color _from;
        [SerializeField] private Color _to;
        [SerializeField] private Image _target;

        private Color _initialValue;

        public override void ReadCurrentToInitial()
        {
            _initialValue = _target.color;
        }

        public override void SetCurrentToInitial()
        {
            _target.color = _initialValue;
        }

        protected override void OnUpdateInternal(float t)
        {
            _target.color = Color.Lerp(_from, _to, t);
        }
    }
}