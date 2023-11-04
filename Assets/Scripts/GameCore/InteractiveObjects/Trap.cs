using GameCore.Character.Movement;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public abstract class Trap : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CharacterMovement characterMovement))
            {
            }
        }
    }
}