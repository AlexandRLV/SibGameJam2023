using UnityEngine;

namespace GameCore.Input
{
    public class InputState
    {
        public Vector2 MoveVector;
        public Vector2 Camera;

        public PressState Jump;
        public PressState Attack;
        public PressState Interact;
        public PressState ChangeCharacter;

        public void Clear()
        {
            MoveVector = Vector2.zero;
            Camera = Vector2.zero;

            Jump = PressState.Released;
            Attack = PressState.Released;
            Interact = PressState.Released;
            ChangeCharacter = PressState.Released;
        }
    }
}