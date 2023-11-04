using GameCore.Input;

namespace Common
{
    public static class Extensions
    {
        public static bool IsReleased(this PressState pressState) => pressState == PressState.Released;
        public static bool IsDown(this PressState pressState) => pressState == PressState.Down;
        public static bool IsHold(this PressState pressState) => pressState is PressState.Down or PressState.Hold;
        public static bool IsUp(this PressState pressState) => pressState == PressState.Up;
    }
}