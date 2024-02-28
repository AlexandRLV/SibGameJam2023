using UnityEngine;

namespace UI.Tween.Tweens
{
    public class TweenAnchoredPosition : TweenBase
    {
        [SerializeField] private Vector2 _from;
        [SerializeField] private Vector2 _to;
        [SerializeField] private RectTransform _target;

        private Vector2 _initial;
        
        public override void ReadCurrentToInitial()
        {
            _initial = _target.anchoredPosition;
        }

        public override void SetCurrentToInitial()
        {
            _target.anchoredPosition = _initial;
        }

        protected override void OnUpdateInternal(float t)
        {
            _target.anchoredPosition = Vector2.Lerp(_from, _to, t);
        }
    }
}