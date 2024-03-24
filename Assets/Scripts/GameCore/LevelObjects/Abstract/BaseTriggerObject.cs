using Common.DI;
using GameCore.Character.Movement;
using GameCore.Sounds;
using GameCore.Sounds.Playback;
using UnityEngine;

namespace GameCore.LevelObjects.Abstract
{
    public abstract class BaseTriggerObject : MonoBehaviour
    {
        [SerializeField] protected SoundType sound;

        protected CharacterMovement Movement;
        
        public bool IsUsed { get; protected set; }
        public bool IsSeen { get; protected set; }

        [Inject] protected SoundService soundService;
        
        private void Awake()
        {
            GameContainer.InjectToInstance(this);
        }

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
            
            if (Movement == movement)
                Movement = null;
        }

        protected virtual void OnPlayerEnter() { }

        protected virtual void OnPlayerStay() { }

        protected virtual void OnPlayerExit() { }
    }
}