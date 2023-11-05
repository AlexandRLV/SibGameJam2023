using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class ToxicPuddle:BaseTriggerObject
    {
        [SerializeField] private float speedMultiplier;
        [SerializeField] private float speedMultiplierDuration;
        
        protected override void OnPlayerEnter()
        {
            Movement.ChangeMovementSpeed(speedMultiplier, speedMultiplierDuration);
        }

        protected override void OnPlayerStay()
        {
           
        }

        protected override void OnPlayerExit()
        {
           
        }
    }
}