using Common;
using Common.DI;
using GameCore.Character.Movement;
using GameCore.Common;
using GameCore.Sounds;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public abstract class BaseTriggerObject : MonoBehaviour
    {
        [SerializeField] protected SoundType sound;

        protected CharacterMovement Movement;
        
        public bool IsUsed { get; protected set; }
        public bool IsSeen { get; protected set; }
        
        protected RoundController RoundController => GameContainer.InGame.Resolve<RoundController>();
        protected SoundService SoundService => GameContainer.Common.Resolve<SoundService>();

        protected void OnTriggerEnter(Collider other)
        {
            var movement = other.GetComponentInParent<CharacterMovement>();
            if (movement == null) return;

            Movement = movement;
            OnPlayerEnter();
        }

        protected void OnTriggerStay(Collider other)
        {
            if (Movement != null)
                OnPlayerStay();
        }

        protected void OnTriggerExit(Collider other)
        {
            var movement = other.GetComponentInParent<CharacterMovement>();
            if (movement == null) return;
            OnPlayerExit();
            Movement = null;
        }

        protected virtual void OnPlayerEnter() { }

        protected virtual void OnPlayerStay() { }

        protected virtual void OnPlayerExit() { }
    }
}