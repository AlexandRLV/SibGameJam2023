namespace GameCore.Character.Visuals
{
    public interface IAnimationSource
    {
        public AnimationType CurrentAnimation { get; }
        public float AnimationSpeed { get; }
    }
}