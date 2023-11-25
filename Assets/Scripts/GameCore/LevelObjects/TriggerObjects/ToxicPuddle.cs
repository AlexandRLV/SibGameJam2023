using Common.DI;
using GameCore.LevelObjects.Abstract;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;

namespace GameCore.LevelObjects.TriggerObjects
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

            if (!canAttackPlayer) return;
            
            canAttackPlayer = false;
            timer = damageTimeRate;
            Movement.Damage();

            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            if (windowsSystem.TryGetWindow<InGameUI>(out var inGameUI))
                inGameUI.SetPoisonState(true);
        }

        protected override void OnPlayerStay()
        {
            Movement.ChangeMovementSpeed(speedMultiplier, speedMultiplierDuration);

            if (!canAttackPlayer) return;
            
            canAttackPlayer = false;
            timer = damageTimeRate;
            Movement.Damage();
        }

        protected override void OnPlayerExit()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            if (windowsSystem.TryGetWindow<InGameUI>(out var inGameUI))
                inGameUI.SetPoisonState(false);
        }

        private void Update()
        {
            if (canAttackPlayer)
                return;
            
            timer -= Time.deltaTime;
            if (timer <= 0) canAttackPlayer = true;
        }
    }
}