﻿using GameCore.Character.Animation;
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
            movement.KnockdownEffect.SetActive(true);
            movement.KnockdownEffect.GetComponent<ParticleSystem>().Play();
        }

        public override void OnExit(MovementStateType nextState)
        {
            moveValues.IsHit = false;
            movement.KnockdownEffect.SetActive(false);
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            movement.Move(Vector2.zero);
        }
    }
}