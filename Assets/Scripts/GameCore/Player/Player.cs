using System.Collections.Generic;
using Common;
using GameCore.Camera;
using GameCore.Character.Movement;
using GameCore.Input;
using UnityEngine;

namespace GameCore.Player
{
    public class Player : MonoBehaviour
    {
        private InputState _inputState;
        private GameCamera _gameCamera;

        private CharacterMovement _currentCharacter;
        private List<CharacterMovement> _characters = new();

        public void RegisterPosessableMovement(CharacterMovement movement)
        {
            _characters.Add(movement);
        }

        public void Initialize()
        {
            _inputState = GameContainer.InGame.Resolve<InputState>();
            _gameCamera = GameContainer.InGame.Resolve<GameCamera>();
            if (_characters.Count == 0) return;
            
            PosessCharacter(_characters[0]);
        }

        private void Update()
        {
            if (!_inputState.changeCharacter.IsDown()) return;

            int currentIndex = _characters.IndexOf(_currentCharacter);
            currentIndex++;
            currentIndex %= _characters.Count;
            PosessCharacter(_characters[currentIndex]);
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