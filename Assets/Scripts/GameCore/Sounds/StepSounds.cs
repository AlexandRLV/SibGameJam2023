using UnityEngine;

namespace GameCore.Sounds
{
    public class StepSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _jumpClip;
        [SerializeField] private AudioClip _landClip;

        [SerializeField] private AudioClip[] _steps;
        
        public void Jump()
        {
            _audioSource.PlayOneShot(_jumpClip);
        }

        public void Land()
        {
            _audioSource.PlayOneShot(_landClip);
        }
        
        private void Step()
        {
            if (!_audioSource.enabled) return;
            
            var clip = _steps[Random.Range(0, _steps.Length)];
            _audioSource.PlayOneShot(clip);
        }
    }
}