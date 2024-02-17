using System.Collections;
using Common;
using Common.DI;
using GameCore.Camera;
using GameCore.Character.Movement;
using GameCore.Common;
using GameCore.Common.Messages;
using GameCore.Input;
using GameCore.RoundControl;
using LocalMessages;
using UnityEngine;

namespace GameCore.Player
{
    public class TwoMousePlayer : MonoBehaviour, IPlayer
    {
        private const float MinChangeMouseTime = 0.5f;
        
        public PlayerMouseType MouseType { get; private set; }
        public CharacterMovement CurrentMovement { get; private set; }
        
        [Inject] private GameCamera _gameCamera;
        [Inject] private LocalMessageBroker _messageBroker;
        [Inject] private InputState _inputState;

        private RoundController _roundController;
        private CharacterMovement _fatMouseCharacter;
        private CharacterMovement _thinMouseCharacter;

        private bool _canChangeMouse;
        private float _changeMouseTimer;
        
        // Not injecting round controller, because it doesn't exist yet
        private IEnumerator Start()
        {
            while (!GameContainer.InGame.CanResolve<RoundController>())
            {
                yield return null;
            }

            _roundController = GameContainer.InGame.Resolve<RoundController>();
        }

        private void Update()
        {
            if (_roundController == null) return;
            if (!_canChangeMouse) return;
            
            _changeMouseTimer += Time.deltaTime;
            if (_changeMouseTimer < MinChangeMouseTime) return;
            if (_roundController.Stage != RoundStage.Game) return;
            if (!_inputState.changeCharacter.IsDown()) return;

            _changeMouseTimer = 0f;
            PosessCharacter(CurrentMovement == _fatMouseCharacter ? _thinMouseCharacter : _fatMouseCharacter);
                
            var message = new ChangeCharacterMessage
            {
                isThinMouse = MouseType == PlayerMouseType.ThinMouse
            };
            _messageBroker.Trigger(ref message);
        }

        public void Initialize(CharacterMovement fatMouse, CharacterMovement thinMouse, bool canChangeCharacter = true)
        {
            _fatMouseCharacter = fatMouse;
            _thinMouseCharacter = thinMouse;
            _canChangeMouse = canChangeCharacter;
            
            _thinMouseCharacter.Unposess();
            PosessCharacter(_fatMouseCharacter);
        }

        public void Unposess()
        {
            if (CurrentMovement != null)
                CurrentMovement.Unposess();

            CurrentMovement = null;
        }

        private void PosessCharacter(CharacterMovement movement)
        {
            if (CurrentMovement != null)
                CurrentMovement.Unposess();

            if (movement == _fatMouseCharacter)
                MouseType = PlayerMouseType.FatMouse;
            else if (movement == _thinMouseCharacter)
                MouseType = PlayerMouseType.ThinMouse;
            
            CurrentMovement = movement;
            CurrentMovement.Posess();
            _gameCamera.SetTarget(CurrentMovement.transform);
        }
    }
}