using GameCore.LevelObjects.Abstract;

namespace GameCore.Character.Movement
{
    public class CharacterMoveValues
    {
        public float SpeedMultiplier;

        public bool IsGrounded;
        public float DistanceToGround;

        public bool IsKnockdown;
        public bool IsHit;
        public bool ForceInteract;
        public InteractiveObject CurrentInteractiveObject;
    }
}