using System.Collections.Generic;
using Common;
using GameCore.Camera;
using GameCore.Character.Movement;
using GameCore.Input;
using UnityEngine;

namespace GameCore.Player
{
    public class GamePlayer : MonoBehaviour
    {
        public List<CharacterMovement> Characters => _characters;
        
        private InputState _inputState;
        private GameCamera _gameCamera;

        private CharacterMovement _currentCharacter;
        private CharacterMovement _fatMouseCharacter;
        private CharacterMovement _thinMouseCharacter;
        private List<CharacterMovement> _characters = new();

        public void Initialize(CharacterMovement fatMouse, CharacterMovement thinMouse)
        {
            _fatMouseCharacter = fatMouse;
            _thinMouseCharacter = thinMouse;

            _characters = new List<CharacterMovement>
            {
                _fatMouseCharacter,
                _thinMouseCharacter
            };
                
            _inputState = GameContainer.InGame.Resolve<InputState>();
            _gameCamera = GameContainer.InGame.Resolve<GameCamera>();
        }

        public void PosessFatMouse() => PosessCharacter(_fatMouseCharacter);
        public void PosessThinMouse() => PosessCharacter(_thinMouseCharacter);

        public void UnPosessAll()
        {
            if (_currentCharacter != null)
                _currentCharacter.Unposess();

            _currentCharacter = null;
        }

        private void PosessCharacter(CharacterMovement movement)
        {
            if (_currentCharacter != null)
                _currentCharacter.Unposess();

            _currentCharacter = movement;
            _currentCharacter.Posess();
            _gameCamera.SetTarget(_currentCharacter.transform);
        }
    }
}