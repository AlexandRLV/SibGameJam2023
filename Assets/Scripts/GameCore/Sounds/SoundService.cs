using System.Collections;
using UnityEngine;

namespace GameCore.Sounds
{
    public class SoundService : MonoBehaviour
    {
        private float _fadeTime;
        private AudioSource _first;
        private AudioSource _second;

        private Coroutine _fadeSourcesRoutine;
        
        private void FadeToMusic(AudioClip clip)
        {
            _second.volume = 0f;
            _second.clip = clip;
            _second.Play();

            if (_fadeSourcesRoutine != null)
                StopCoroutine(_fadeSourcesRoutine);
            
            _fadeSourcesRoutine = StartCoroutine(FadeSources());
        }

        private IEnumerator FadeSources()
        {
            float time = 0f;
            while (time < _fadeTime)
            {
                float t = time / _fadeTime;
                _first.volume = Mathf.Lerp(1f, 0f, t);
                _second.volume = Mathf.Lerp(0f, 1f, t);
                time += Time.deltaTime;
                yield return null;
            }

            _first.volume = 0f;
            _second.volume = 1f;
            
            _first.Stop();
            
            (_first, _second) = (_second, _first);
            _fadeSourcesRoutine = null;
        }
    }
}