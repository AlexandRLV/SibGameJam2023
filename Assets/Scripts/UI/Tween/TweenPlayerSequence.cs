using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Tween
{
    public class TweenPlayerSequence : MonoBehaviour
    {
        private bool IsEnded => _reverse ? _currentPlayerId < 0 : _currentPlayerId >= _tweenPlayers.Count;
        
        private TweenPlayer Current =>
            _currentPlayerId < 0 || _currentPlayerId >= _tweenPlayers.Count
                ? null
                : _tweenPlayers[_currentPlayerId];

        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _initialOnDisable = true;
        [SerializeField] private List<TweenPlayer> _tweenPlayers;

        private bool _reverse;
        private float _currentDelay;

        private int _currentPlayerId = -1;
        private Coroutine _sequenceRoutine;

        private void OnEnable()
        {
            foreach (var tweenPlayer in _tweenPlayers)
            {
                tweenPlayer.playOnEnable = false;
                tweenPlayer.initialOnDisable = false;
            }
            
            if (_playOnEnable) Play();
        }

        private void OnDisable()
        {
            if (_initialOnDisable) SetInitial();
        }

        public void Play(bool reverse = false)
        {
            SetInitial();
            
            _reverse = reverse;
            _currentDelay = 0f;
            _currentPlayerId = reverse ? _tweenPlayers.Count : -1;
            _sequenceRoutine = StartCoroutine(Sequence());
        }

        public void Stop()
        {
            var current = Current;
            if (current != null) current.Stop();
            
            if (_sequenceRoutine == null) return;
            
            StopCoroutine(_sequenceRoutine);
            _sequenceRoutine = null;
        }

        public void SetInitial()
        {
            Stop();
            for (int i = _tweenPlayers.Count - 1; i >= 0; i--)
            {
                _tweenPlayers[i].SetInitial();
            }
        }

        public void SetFinal()
        {
            Stop();
            foreach (var tweenPlayer in _tweenPlayers)
            {
                tweenPlayer.SetFinal();
            }
        }

        private IEnumerator Sequence()
        {
            while (!IsEnded)
            {
                while (_currentDelay > 0)
                {
                    _currentDelay -= Time.unscaledDeltaTime;
                    yield return null;
                }

                if (_reverse) _currentPlayerId--;
                else _currentPlayerId++;

                var current = Current;
                if (current == null) continue;

                _currentDelay = current.duration;
                current.Play(_reverse);
            }
        }
        
    }
}