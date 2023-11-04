using System;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class EatableObject : MonoBehaviour
    {
        public event Action OnInteracted;
        
        [SerializeField] private float speedMultiplier;
        [SerializeField] private float speedMultiplierDuration;

        public float SpeedMultiplier => speedMultiplier;

        public float SpeedMultiplierDuration => speedMultiplierDuration;

        public void Interact()
        {
            OnInteracted?.Invoke();
        }
    }
}