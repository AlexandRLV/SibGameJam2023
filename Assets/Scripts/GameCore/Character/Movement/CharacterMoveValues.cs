using GameCore.InteractiveObjects;

namespace GameCore.Character.Movement
{
    public class CharacterMoveValues
    {
        public float SpeedMultiplier;
        public float JumpHeightMultiplier;
        public float FloatingHeightMultiplier;

        public bool IsKnockdown;
        public bool IsHit;
        
        public InteractiveObject CurrentInteractiveObject;
    }
}