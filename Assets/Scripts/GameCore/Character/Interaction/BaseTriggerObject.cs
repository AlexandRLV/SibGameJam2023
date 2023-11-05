using GameCore.Character.Movement;
using UnityEngine;

namespace GameCore.Character.Interaction
{
    public abstract class BaseTriggerObject : MonoBehaviour
    {
        protected CharacterMovement enteredMovement;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out CharacterMovement movement)) return;

            enteredMovement = movement;
            OnCharacterEnter();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out CharacterMovement _)) return;

            enteredMovement = null;
            OnCharacterExit();
        }

        protected abstract void OnCharacterEnter();
        protected virtual void OnCharacterExit() { }
    }
}