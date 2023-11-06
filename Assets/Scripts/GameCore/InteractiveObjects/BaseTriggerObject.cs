using Common;
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
            OnPlayerStay();
        }

        protected void OnTriggerExit(Collider other)
        {
            var movement = other.GetComponentInParent<CharacterMovement>();
            if (movement == null) return;
            OnPlayerExit();
            Movement = null;
        }

        protected abstract void OnPlayerEnter();

        protected abstract void OnPlayerStay();

        protected abstract void OnPlayerExit();
    }
}