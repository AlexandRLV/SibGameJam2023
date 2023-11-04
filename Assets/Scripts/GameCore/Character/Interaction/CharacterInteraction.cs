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

        private void Start()
        {
            _notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
        }

        private void Update()
        {
            if (characterMovement.IsControlledByPlayer && _currentEatableObject &&
                characterMovement.InputState.interact.IsDown())
            {
                _currentEatableObject.Interact();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out EatableObject eatableObject))
            {
                _currentEatableObject = eatableObject;
                eatableObject.OnInteracted += () =>
                    characterMovement.ChangeMovementSpeed(_currentEatableObject.SpeedMultiplier,
                        eatableObject.SpeedMultiplierDuration);
            }
        }

        private void OnTriggerStay(Collider other)
        {
        }

        private void OnTriggerExit(Collider other)
        {
            _currentEatableObject = null;
        }
    }
}