using GameCore.Character.Animation;
using GameCore.Character.Movement;
using GameCore.Sounds;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public abstract class BaseTriggerObject : MonoBehaviour
    {
        [SerializeField] protected SoundType sound;

        protected CharacterMovement Movement;

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
            if (!other.TryGetComponent(out CharacterMovement _)) return;
            OnPlayerExit();
            Movement = null;
        }

        protected abstract void OnPlayerEnter();

        protected abstract void OnPlayerStay();

        protected abstract void OnPlayerExit();
    }
}