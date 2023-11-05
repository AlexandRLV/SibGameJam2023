using GameCore.Input;
using UnityEngine;

namespace Common
{
    public static class Extensions
    {
        public static bool IsReleased(this PressState pressState) => pressState == PressState.Released;
        public static bool IsDown(this PressState pressState) => pressState == PressState.Down;
        public static bool IsHold(this PressState pressState) => pressState is PressState.Down or PressState.Hold;
        public static bool IsUp(this PressState pressState) => pressState == PressState.Up;

        public static Vector3 WithY(this Vector3 origin, float y)
        {
            origin.y = y;
            return origin;
        }

        public static Quaternion FlatRotation(this Transform transform)
        {
            float eulerY = transform.eulerAngles.y;
            return Quaternion.Euler(0f, eulerY, 0f);
        }

        public static Vector3 FlatVector(this Vector3 value)
        {
            value.y = 0f;
            return value;
        }
    }
}