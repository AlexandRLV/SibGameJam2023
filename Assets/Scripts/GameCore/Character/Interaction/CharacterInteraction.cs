using Common;
using GameCore.Character.Movement;
using GameCore.InteractiveObjects;
using UnityEngine;

namespace GameCore.Character.Interaction
{
    public class CharacterInteraction : MonoBehaviour
    {
        [SerializeField] private CharacterMovement characterMovement;

        private void Update()
        {
            if (characterMovement.IsControlledByPlayer &&
                characterMovement.InputState.interact.IsDown())
            {
                if (characterMovement.MoveValues.EatableObject != null)
                {
                    characterMovement.ChangeMovementSpeed(characterMovement.MoveValues.EatableObject.SpeedMultiplier,
                    characterMovement.MoveValues.EatableObject.SpeedMultiplierDuration);
                }
                else if (characterMovement.MoveValues.PrisonController != null)
                {
                    characterMovement.MoveValues.PrisonController.OpenDoor();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out EatableObject eatableObject))
            {
                characterMovement.MoveValues.EatableObject = eatableObject;
            }
            else if (other.TryGetComponent(out PrisonController prisonDoor))
            {
                characterMovement.MoveValues.PrisonController = prisonDoor;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out EatableObject _))
            {
                characterMovement.MoveValues.EatableObject = null;
            }
            else if (other.TryGetComponent(out PrisonController _))
            {
                characterMovement.MoveValues.PrisonController = null;
            }
        }
    }
}