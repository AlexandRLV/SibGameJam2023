using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class ToxicPuddle : BaseTriggerObject
    {
        [SerializeField] private float speedMultiplier;
        [SerializeField] private float speedMultiplierDuration;
        [Header("Damage")]
        [SerializeField] private float damageTimeRate;

        private bool canAttackPlayer = true;
        private float timer;

        protected override void OnPlayerEnter()
        {
            Movement.ChangeMovementSpeed(speedMultiplier, speedMultiplierDuration);

            if (canAttackPlayer == true)
            {
                canAttackPlayer = false;
                timer = damageTimeRate;
                Movement.Damage();
            }

        }

        protected override void OnPlayerStay()
        {
            Movement.ChangeMovementSpeed(speedMultiplier, speedMultiplierDuration);

            if (canAttackPlayer == true)
            {
                canAttackPlayer = false;
                timer = damageTimeRate;
                Movement.Damage();
            }
                
        }

        protected override void OnPlayerExit()
        {

        }

        private void Update()
        {
            if (canAttackPlayer == false) Timer();
        }

        private void Timer()
        {
            timer -= Time.deltaTime;
            if (timer <= 0) canAttackPlayer = true;
        }
    }
}