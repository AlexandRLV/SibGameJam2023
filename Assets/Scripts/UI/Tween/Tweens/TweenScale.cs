using UnityEngine;

namespace UI.Tween.Tweens
{
    public class TweenScale : TweenBase
    {
        [SerializeField] private Vector3 _from;
        [SerializeField] private Vector3 _to;
        [SerializeField] private Transform _target;

        private Vector3 _initial;

        public override void ReadCurrentToInitial()
        {
            _initial = _target.localScale;
        }

        public override void SetCurrentToInitial()
        {
            _target.localScale = _initial;
        }

        protected override void OnUpdateInternal(float t)
        {
            _target.localScale = Vector3.Lerp(_from, _to, t);
        }
    }
}