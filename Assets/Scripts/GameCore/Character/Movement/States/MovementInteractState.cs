using Common;
using Common.DI;
using GameCore.Character.Animation;
using NetFrame.Client;
using Networking;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Character.Movement.States
{
    internal class MovementInteractState : MovementStateBase
    {
        public override AnimationType AnimationType => _animationType;
        public override MovementStateType Type => MovementStateType.Interact;

        private AnimationType _animationType;
        private float _timer;
        
        public MovementInteractState(CharacterMovement characterMovement) : base(characterMovement)
        {
        }

        public override bool CanEnter(MovementStateType prevState)
        {
            if (!movement.IsControlledByPlayer) return false;
            if (!movement.InputState.interact.IsDown() && !moveValues.ForceInteract) return false;

            return moveValues.CurrentInteractiveObject != null;
        }

        public override bool CanExit(MovementStateType nextState)
        {
            return _timer <= 0f;
        }

        public override void OnEnter(MovementStateType prevState)
        {
            var interactiveObject = moveValues.CurrentInteractiveObject;
            _animationType = interactiveObject.InteractAnimation;
            moveValues.ForceInteract = false;
            _timer = parameters.interactTime;
            interactiveObject.Interact();

            var gameClient = GameContainer.Common.Resolve<GameClient>();
            if (!gameClient.IsConnected) return;

            var dataframe = new InteractedWithObjectDataframe
            {
                interactedObject = interactiveObject.Type,
                objectPosition = interactiveObject.transform.position
            };
            GameContainer.Common.Resolve<NetFrameClient>().Send(ref dataframe);
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            movement.Move(Vector2.zero);
        }
    }
}