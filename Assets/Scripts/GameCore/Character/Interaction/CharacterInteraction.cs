using System;
using Common;
using GameCore.Character.Movement;
using GameCore.InteractiveObjects;
using UI.NotificationsSystem;
using UnityEngine;

namespace GameCore.Character.Interaction
{
    public class CharacterInteraction : MonoBehaviour
    {
        [SerializeField] private CharacterMovement characterMovement;
        private NotificationsManager _notificationsManager;

        private EatableObject _currentEatableObject;
        private PrisonController _prisonDoorController;

        private void Start()
        {
            _notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
        }

        private void Update()
        {
            if (characterMovement.IsControlledByPlayer &&
                characterMovement.InputState.interact.IsDown())
            {
                if (_currentEatableObject != null)
                {
                    characterMovement.ChangeMovementSpeed(_currentEatableObject.SpeedMultiplier,
                    _currentEatableObject.SpeedMultiplierDuration);
                }
                else if (_prisonDoorController != null)
                {
                    _prisonDoorController.OpenDoor();
                }

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out EatableObject eatableObject))
            {
                _currentEatableObject = eatableObject;

            }
            else if (other.TryGetComponent(out PrisonController prisonDoor))
            {
                _prisonDoorController = prisonDoor;
            }
        }

        private void OnTriggerStay(Collider other)
        {
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out EatableObject eatableObject))
            {
                _currentEatableObject = null;

            }
            else if (other.TryGetComponent(out PrisonController prisonDoor))
            {
                _prisonDoorController = null;
            }
        }
    }
}