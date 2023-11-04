using UnityEngine;

namespace GameCore.Input
{
    [CreateAssetMenu(fileName = "Input Key Settings")]
    public class InputKeySettings : ScriptableObject
    {
        public KeyCode ForwardMoveKey;
        public KeyCode BackMoveKey;
        public KeyCode RightMoveKey;
        public KeyCode LeftMoveKey;
        
        public KeyCode JumpKey;
        public KeyCode AttackKey;
        public KeyCode InteractKey;
    }
}