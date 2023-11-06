using System.Collections.Generic;
using Common;
using GameCore.Camera;
using GameCore.Character.Movement;
using UnityEngine;

namespace GameCore.Player
{
    public class GamePlayer : MonoBehaviour
    {
        public CharacterMovement CurrentCharacter { get; private set; }

        private GameCamera _gameCamera;

        private CharacterMovement _fatMouseCharacter;
        // private CharacterMovement _thinMouseCharacter;

        public void Initialize(CharacterMovement fatMouse)
        {
            _fatMouseCharacter = fatMouse;
            // _thinMouseCharacter = thinMouse;
                
            _gameCamera = GameContainer.InGame.Resolve<GameCamera>();
            
            _fatMouseCharacter.Unposess();
            // _thinMouseCharacter.Unposess();
        }

        public void PosessFatMouse() => PosessCharacter(_fatMouseCharacter);
        // public void PosessThinMouse() => PosessCharacter(_thinMouseCharacter);

        // public void PosessAnother() =>
        //     PosessCharacter(CurrentCharacter == _fatMouseCharacter ? _thinMouseCharacter : _fatMouseCharacter);

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

            CurrentCharacter = movement;
            CurrentCharacter.Posess();
            _gameCamera.SetTarget(CurrentCharacter.transform);
        }
    }
}