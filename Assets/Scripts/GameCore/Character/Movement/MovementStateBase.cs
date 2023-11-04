using GameCore.StateMachine;
using UnityEngine;

namespace GameCore.Character.Movement
{
    public abstract class MovementStateBase : StateBase<MovementStateType>
    {
        protected readonly CharacterMovement movement;
        protected readonly CharacterParameters parameters;
        protected readonly Rigidbody rigidbody;
        protected readonly CharacterMoveValues moveValues;

        protected MovementStateBase(CharacterMovement characterMovement)
        {
            movement = characterMovement;
            parameters = characterMovement.Parameters;
            rigidbody = characterMovement.Rigidbody;
            moveValues = characterMovement.MoveValues;
        }
    }
}