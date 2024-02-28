using System.Collections.Generic;
using UnityEngine;

namespace UI.Tween
{
    public class TweenPlayer : MonoBehaviour
    {
        [SerializeField] public bool playOnEnable = true;
        [SerializeField] public bool initialOnDisable = true;
        [SerializeField] public float duration = 1f;
        
        [SerializeField] private List<TweenBase> _tweens;

        private void OnEnable()
        {
            if (playOnEnable)
                Play();
        }

        private void OnDisable()
        {
            if (initialOnDisable)
                SetInitial();
        }

        public void Play(bool reverse = false)
        {
            if (_tweens == null || _tweens.Count == 0) return;

            foreach (var tween in _tweens)
            {
                if (tween != null)
                    tween.StartTween(reverse);
            }
        }

        public void Stop()
        {
            if (_tweens == null || _tweens.Count == 0) return;

            foreach (var tween in _tweens)
            {
                if (tween != null)
                    tween.StopTween();
            }
        }

        public void ReadInitial()
        {
            if (_tweens == null || _tweens.Count == 0) return;

            foreach (var tween in _tweens)
            {
                if (tween != null)
                    tween.ReadCurrentToInitial();
            }
        }

        public void SetInitial()
        {
            if (_tweens == null || _tweens.Count == 0) return;

            foreach (var tween in _tweens)
            {
                if (tween != null)
                    tween.SetCurrentToInitial();
            }
        }

        public void SetFinal()
        {
            if (_tweens == null || _tweens.Count == 0) return;

            foreach (var tween in _tweens)
            {
                if (tween != null)
                    tween.SetFinal();
            }
        }
    }
}