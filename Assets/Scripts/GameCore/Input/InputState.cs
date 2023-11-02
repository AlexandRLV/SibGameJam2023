using UnityEngine;

namespace GameCore.Input
{
    public class InputState
    {
        public Vector2 Movement;
        public Vector2 Camera;

        public bool JumpPressed;
        public bool AttackPressed;
        public bool InteractPressState;

        public void Clear()
        {
            Movement = Vector2.zero;
            Camera = Vector2.zero;

            JumpPressed = false;
            AttackPressed = false;
            InteractPressState = false;
        }
    }
}