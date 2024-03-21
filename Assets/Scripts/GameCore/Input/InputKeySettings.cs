using UnityEngine;

namespace GameCore.Input
{
    [CreateAssetMenu(menuName = "Configs/Input Key Settings")]
    public class InputKeySettings : ScriptableObject
    {
        public KeyCode forwardMoveKey;
        public KeyCode backMoveKey;
        public KeyCode rightMoveKey;
        public KeyCode leftMoveKey;
        
        public KeyCode jumpKey;
        public KeyCode crouchKey;
        public KeyCode interactKey;
        public KeyCode changeCharacterKey;
        public KeyCode cheatSpeedUpKey;
    }
}