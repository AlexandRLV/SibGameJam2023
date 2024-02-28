using UnityEngine;

namespace UI.Tween.Tweens
{
    public class TweenEnable : TweenBase
    {
        [SerializeField] private bool _finalValue;
        [SerializeField] private GameObject _target;

        protected override void OnUpdateInternal(float t)
        {
            if (t >= duration)
                _target.SetActive(_finalValue);
        }
    }
}