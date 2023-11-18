using Common;
using GameCore.Camera;
using GameCore.Character.Movement;
using UnityEngine;

namespace GameCore.Player
{
    public class GamePlayer : MonoBehaviour
    {
        public PlayerMouseType MouseType { get; private set; }
        public CharacterMovement CurrentCharacter { get; private set; }

        private GameCamera _gameCamera;

        private CharacterMovement _fatMouseCharacter;
        private CharacterMovement _thinMouseCharacter;

        public void Initialize(CharacterMovement fatMouse, CharacterMovement thinMouse)
        {
            _fatMouseCharacter = fatMouse;
            _thinMouseCharacter = thinMouse;
                
            _gameCamera = GameContainer.InGame.Resolve<GameCamera>();
            
            _thinMouseCharacter.Unposess();
            PosessCharacter(_fatMouseCharacter);
        }

        public void PosessAnother() =>
            PosessCharacter(CurrentCharacter == _fatMouseCharacter ? _thinMouseCharacter : _fatMouseCharacter);

        public void UnposessAll()
        {
            if (CurrentCharacter != null)
                CurrentCharacter.Unposess();

            CurrentCharacter = null;
        }

        private void PosessCharacter(CharacterMovement movement)
        {
            if (CurrentCharacter != null)
                CurrentCharacter.Unposess();

            if (movement == _fatMouseCharacter)
                MouseType = PlayerMouseType.FatMouse;
            else if (movement == _thinMouseCharacter)
                MouseType = PlayerMouseType.ThinMouse;
            
            CurrentCharacter = movement;
            CurrentCharacter.Posess();
            _gameCamera.SetTarget(CurrentCharacter.transform);
        }
    }
}