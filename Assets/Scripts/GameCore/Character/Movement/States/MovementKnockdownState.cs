using GameCore.Character.Animation;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Character.Movement.States
{
    public class MovementKnockdownState : MovementStateBase
    {
        public override AnimationType AnimationType => AnimationType.Knockdown;
        public override MovementStateType Type => MovementStateType.Knockdown;

        public MovementKnockdownState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            return moveValues.isKnockdown;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return !moveValues.isKnockdown;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            if (!movement.GameClientData.IsConnected) return;

            var dataframe = new PlayerEffectStateDataframe
            {
                type = (byte) EffectType.Knockdown,
                active = true,
            };
            movement.GameClient.Send(ref dataframe);
        }

        public override void Update()
        {
            movement.PhysicsBody.UpdateMovement(Vector2.zero);
        }
    }
}