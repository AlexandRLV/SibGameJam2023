using UnityEngine;
using UnityEngine.UI;

namespace UI.Tween.Tweens
{
    public class TweenAlphaGraphic : TweenBase
    {
        [SerializeField] private float _from;
        [SerializeField] private float _to;
        [SerializeField] private Graphic _target;

        private float _initialValue;

        public override void ReadCurrentToInitial()
        {
            _initialValue = _target.color.a;
        }

        public override void SetCurrentToInitial()
        {
            SetValue(_initialValue);
        }

        protected override void OnUpdateInternal(float t)
        {
            SetValue(Mathf.Lerp(_from, _to, t));
        }

        private void SetValue(float a)
        {
            var color = _target.color;
            color.a = a;
            _target.color = color;
        }
    }
}