using GameCore.Character.Animation;
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
            return moveValues.IsKnockdown;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return !moveValues.IsKnockdown;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            movement.KnockdownEffect.SetActive(true);
            movement.KnockdownEffect.GetComponent<ParticleSystem>().Play();
        }

        public override void Update()
        {
            movement.Move(Vector2.zero);
        }
    }
}