using System.Runtime.CompilerServices;
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

        public static void ToLocalZero(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

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
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(this Vector3 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinity(this Vector3 v)
        {
            return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(this Quaternion v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z) || float.IsNaN(v.w);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinity(this Quaternion v)
        {
            return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z) || float.IsInfinity(v.w);
        }
    }
}