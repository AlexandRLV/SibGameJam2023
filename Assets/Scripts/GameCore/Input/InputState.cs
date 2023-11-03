using UnityEngine;

namespace GameCore.Input
{
    public class InputState
    {
        public Vector2 MoveVector;
        public Vector2 Camera;

        public bool JumpPressed;
        public bool AttackPressed;
        public bool InteractPressState;

        public void Clear()
        {
            MoveVector = Vector2.zero;
            Camera = Vector2.zero;

            JumpPressed = false;
            AttackPressed = false;
            InteractPressState = false;
        }
    }
}