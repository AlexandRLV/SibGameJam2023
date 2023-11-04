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
            if (UnityEngine.Input.GetKey(_keySettings.ForwardMoveKey))
                movement.y += 1f;
            if (UnityEngine.Input.GetKey(_keySettings.BackMoveKey))
                movement.y -= 1f;
            if (UnityEngine.Input.GetKey(_keySettings.RightMoveKey))
                movement.x += 1f;
            if (UnityEngine.Input.GetKey(_keySettings.LeftMoveKey))
                movement.x -= 1f;

            _inputState.MoveVector = movement;
            _inputState.Camera =
                new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));

            _inputState.JumpPressed = UnityEngine.Input.GetKey(_keySettings.JumpKey);
            _inputState.AttackPressed = UnityEngine.Input.GetKey(_keySettings.AttackKey);
            _inputState.InteractPressState = UnityEngine.Input.GetKey(_keySettings.InteractKey);
        }
    }
}