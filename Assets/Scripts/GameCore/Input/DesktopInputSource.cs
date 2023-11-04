using Common;
using UnityEngine;

namespace GameCore.Input
{
    public class DesktopInputSource : MonoBehaviour
    {
        [SerializeField] private InputKeySettings _keySettings;

        private InputState _inputState;
        
        private void Start()
        {
            _inputState = GameContainer.InGame.Resolve<InputState>();
        }

        private void Update()
        {
            _inputState.Clear();

            var movement = Vector2.zero;
            if (UnityEngine.Input.GetKey(_keySettings.forwardMoveKey))
                movement.y += 1f;
            if (UnityEngine.Input.GetKey(_keySettings.backMoveKey))
                movement.y -= 1f;
            if (UnityEngine.Input.GetKey(_keySettings.rightMoveKey))
                movement.x += 1f;
            if (UnityEngine.Input.GetKey(_keySettings.leftMoveKey))
                movement.x -= 1f;

            _inputState.moveVector = movement;
            _inputState.camera =
                new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));

            _inputState.jump = GetPressState(_keySettings.jumpKey);
            _inputState.crouch = GetPressState(_keySettings.crouchKey);
            _inputState.sneak = GetPressState(_keySettings.sneakKey);
            _inputState.interact = GetPressState(_keySettings.interactKey);
            _inputState.changeCharacter = GetPressState(_keySettings.changeCharacterKey);
        }

        private PressState GetPressState(KeyCode key)
        {
            if (UnityEngine.Input.GetKeyDown(key)) return PressState.Down;
            if (UnityEngine.Input.GetKeyUp(key)) return PressState.Up;
            if (UnityEngine.Input.GetKey(key)) return PressState.Hold;
            return PressState.Released;
        }
    }
}