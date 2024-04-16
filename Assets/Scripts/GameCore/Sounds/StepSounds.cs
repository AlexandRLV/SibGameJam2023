using GameCore.LevelObjects.FloorTypeDetection;
using GameCore.Sounds.Steps;
using UnityEngine;

namespace GameCore.Sounds
{
    public class StepSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _jumpClip;
        [SerializeField] private AudioClip _landClip;

        [SerializeField] private StepSoundsConfig _config;

        [SerializeField] private bool _debug;
        
        private FloorTypeDetector _floorTypeDetector;

        public void Initialize(FloorTypeDetector floorTypeDetector) => _floorTypeDetector = floorTypeDetector;
        public void Jump() => _audioSource.PlayOneShot(_jumpClip);
        public void Land() => _audioSource.PlayOneShot(_landClip);

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                if (_debug)
                {
                    var floorType = _floorTypeDetector.GetCurrentType();
                    var clip = _config.GetRandomClipForFloorType(floorType);
                    if (clip == null)
                    {
                        Debug.Log($"clip is null {transform.root.gameObject.name}");
                        return;
                    }
            
                    Debug.Log($"STEP!!! {transform.root.gameObject.name}");
                    _audioSource.PlayOneShot(clip);
                }
            }
        }

        private void Step()
        {
            if (_debug)
            {
                // awe
            }
            
            if (!_audioSource.enabled) return;
            
            var floorType = _floorTypeDetector.GetCurrentType();
            var clip = _config.GetRandomClipForFloorType(floorType);
            if (clip == null) return;
            
            _audioSource.PlayOneShot(clip);
            
            // var clip = _steps[Random.Range(0, _steps.Length)];
            // _audioSource.PlayOneShot(clip);
        }
    }
}