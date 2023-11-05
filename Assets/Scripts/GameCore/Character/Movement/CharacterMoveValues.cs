using GameCore.Character.Interaction;
using GameCore.InteractiveObjects;

namespace GameCore.Character.Movement
{
    public class CharacterMoveValues
    {
        public float SpeedMultiplier;
        public float JumpHeightMultiplier;
        public float FloatingHeightMultiplier;

        // Knockdown
        public bool IsKnockdown;
        public float KnockdownTime;
        
        public InteractiveObject CurrentInteractiveObject;
    }
}