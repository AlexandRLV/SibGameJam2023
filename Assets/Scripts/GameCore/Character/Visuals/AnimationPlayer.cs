using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Character.Animation
{
    public class AnimationPlayer : MonoBehaviour
    {
        private static readonly Dictionary<AnimationType, int> AnimationIds = new()
        {
            { AnimationType.IdleWait, Animator.StringToHash("IdleWait") },
            { AnimationType.Walk, Animator.StringToHash("Idle") },
            { AnimationType.Crouch, Animator.StringToHash("Crouch") },
            { AnimationType.Jump, Animator.StringToHash("Jump") },
            { AnimationType.Fall, Animator.StringToHash("Fall") },
            { AnimationType.OpenDoor, Animator.StringToHash("OpenDoor") },
            { AnimationType.Eat, Animator.StringToHash("Eat") },
            { AnimationType.Hit, Animator.StringToHash("Hit") },
            { AnimationType.Knockdown, Animator.StringToHash("Knockdown") },
        };

        private static readonly int SpeedId = Animator.StringToHash("Speed");
        
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimatorOverrideController _overrideController;

        private int? _currentAnimation;

        private void Awake()
        {
            if (_overrideController != null)
                _animator.runtimeAnimatorController = _overrideController;
        }

        public void SetAnimation(AnimationType type, bool force = false)
        {
            int stateId = AnimationIds[type];
            if (_currentAnimation == stateId && !force) return;

            _currentAnimation = stateId;
            _animator.CrossFade(stateId, 0.1f);
        }

        public void SetSpeed(float speed)
        {
            _animator.SetFloat(SpeedId, speed);
        }
    }
}