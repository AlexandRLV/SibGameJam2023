namespace GameCore.Character.Animation
{
    public interface IAnimationSource
    {
        public AnimationType CurrentAnimation { get; }
        public float AnimationSpeed { get; }
    }
}