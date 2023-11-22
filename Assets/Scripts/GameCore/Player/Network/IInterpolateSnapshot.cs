using UnityEngine;

namespace GameCore.Player.Network
{
    public interface IInterpolateSnapshot<in T> where T : IInterpolateSnapshot<T>
    {
        public int Tick { get; set; }
        public bool Teleported { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public void InterpolateValues(T from, T to, float lerpAmount);
    }
}