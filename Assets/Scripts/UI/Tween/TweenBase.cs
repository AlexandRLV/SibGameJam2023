using System.Collections;
using UnityEngine;

namespace UI.Tween
{
    public class TweenBase : MonoBehaviour
    {
        [SerializeField] private bool _loop;
        
        [SerializeField] protected float duration = 0.5f;
        [SerializeField] protected AnimationCurve animationCurve = new(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        private bool _reversed;
        private bool _active;
        private float _timeElapsed;

        private Coroutine _tweenRoutine;

        public void StartTween(bool reversed = false)
        {
            _reversed = reversed;
            StopTween();
            _active = true;

            if (gameObject.activeInHierarchy)
                _tweenRoutine = StartCoroutine(Tween());
        }

        public void StopTween()
        {
            _active = false;
            _timeElapsed = 0f;
            
            if (_tweenRoutine == null) return;
            
            StopCoroutine(_tweenRoutine);
            _tweenRoutine = null;
        }

        public void SetFinal()
        {
            StopTween();
            OnUpdateInternal(animationCurve.Evaluate(animationCurve.Evaluate(1)));
        }
        
        public virtual void ReadCurrentToInitial() { }
        public virtual void SetCurrentToInitial() { }

        protected virtual void OnUpdateInternal(float t) { }

        private IEnumerator Tween()
        {
            while (_active && _timeElapsed < duration)
            {
                float t = _timeElapsed / duration;
                if (duration > 0f)
                {
                    t = _reversed ? 1f - t : t;
                    t = animationCurve.Evaluate(t);
                }
                else
                {
                    t = animationCurve.Evaluate(_reversed ? 0f : 1f);
                }
                
                OnUpdateInternal(t);
                _timeElapsed += Time.unscaledDeltaTime;

                if (_timeElapsed > duration && _loop)
                    _timeElapsed = 0f;

                yield return null;
            }
            
            OnUpdateInternal(animationCurve.Evaluate(animationCurve.Evaluate(_reversed ? 0f : 1f)));
            StopTween();
        }
    }
}