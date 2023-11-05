using UnityEngine;

namespace GameCore.Character.Animation
{
    public class CharacterVisuals : MonoBehaviour
    {
        [SerializeField] private AnimationPlayer _animationPlayer;

        private bool _initialized;
        private IAnimationSource _animationSource;

        public void Initialize(IAnimationSource source)
        {
            _animationSource = source;
            _initialized = true;
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