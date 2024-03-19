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
            return moveValues.isHit;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return _timer <= 0f;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            _timer = parameters.hitTime;
            movement.SetEffectState(EffectType.Knockdown, true);

            if (!movement.GameClientData.IsConnected) return;

            var dataframe = new PlayerEffectStateDataframe
            {
                type = (byte)EffectType.Knockdown,
                active = true,
            };
            movement.GameClient.Send(ref dataframe);
        }

        public override void OnExit(MovementStateType nextState)
        {
            moveValues.isHit = false;
            movement.SetEffectState(EffectType.Knockdown, false);
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            movement.PhysicsBody.UpdateMovement(Vector2.zero);
            
            if (!movement.GameClientData.IsConnected) return;

            var dataframe = new PlayerEffectStateDataframe
            {
                type = (byte)EffectType.Knockdown,
                active = false,
            };
            movement.GameClient.Send(ref dataframe);
        }
    }
}