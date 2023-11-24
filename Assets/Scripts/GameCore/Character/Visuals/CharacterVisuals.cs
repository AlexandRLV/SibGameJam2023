using GameCore.Sounds;
using UnityEngine;

namespace GameCore.Character.Animation
{
    public class CharacterVisuals : MonoBehaviour
    {
        public GameObject SpeedUp => _speedUpVFX;
        public StepSounds StepSounds => _stepSounds;
        public GameObject KnockdownEffect => _knockdownEffect;
        
        [SerializeField] private AnimationPlayer _animationPlayer;
        [SerializeField] private GameObject _speedUpVFX;
        [SerializeField] private StepSounds _stepSounds;
        [SerializeField] private GameObject _knockdownEffect;
        
        private bool _initialized;
        private IAnimationSource _animationSource;

        public void Initialize(IAnimationSource source)
        {
            _animationSource = source;
            _initialized = true;
            
            if (_speedUpVFX != null)
                _speedUpVFX.SetActive(false);
            
            if (_knockdownEffect != null)
                _knockdownEffect.SetActive(false);
        }

        private void Update()
        {
            if (!_initialized) return;

            var currentAnimation = _animationSource.CurrentAnimation;
            float speed = _animationSource.AnimationSpeed;
            
            _animationPlayer.SetAnimation(currentAnimation);
            _animationPlayer.SetSpeed(speed);
        }
    }
}