using GameCore.Character.Animation;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Character.Movement.States
{
    public class MovementHitState : MovementStateBase
    {
        public override AnimationType AnimationType => AnimationType.Hit;
        public override MovementStateType Type => MovementStateType.Hit;

        private float _timer;
        
        public MovementHitState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            return moveValues.IsHit;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return _timer <= 0f;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            _timer = parameters.hitTime;
            movement.SetEffectState(EffectType.Knockdown, true);

            if (!movement.GameClient.IsConnected) return;

            var dataframe = new PlayerEffectStateDataframe
            {
                type = EffectType.Knockdown,
                active = true,
            };
            movement.GameClient.Send(ref dataframe);
        }

        public override void OnExit(MovementStateType nextState)
        {
            moveValues.IsHit = false;
            movement.SetEffectState(EffectType.Knockdown, false);
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            movement.Move(Vector2.zero);
            
            if (!movement.GameClient.IsConnected) return;

            var dataframe = new PlayerEffectStateDataframe
            {
                type = EffectType.Knockdown,
                active = false,
            };
            movement.GameClient.Send(ref dataframe);
        }
    }
}