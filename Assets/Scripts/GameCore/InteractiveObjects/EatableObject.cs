using System;
using Common;
using GameCore.Sounds;
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

        private SoundService _soundService = GameContainer.Common.Resolve<SoundService>();

        public void Interact()
        {
            OnInteracted?.Invoke();
            _soundService.PlaySound(SoundType.Buff);
        }
    }
}