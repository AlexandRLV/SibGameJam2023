using Common;
using GameCore.InteractiveObjects;
using UI.NotificationsSystem;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Character.Interaction
{
    public class CharacterInteraction : MonoBehaviour
    {
        private NotificationsManager _notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactiveObject))
            {
                _notificationsManager.ShowNotification("Interact!!", NotificationsManager.NotificationType.Center);
            }
        }

        private void OnTriggerStay(Collider other)
        {
        }

        private void OnTriggerExit(Collider other)
        {
        }
    }
}