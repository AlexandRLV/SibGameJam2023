using System.Collections;
using Common.DI;
using GameCore.Camera;
using GameCore.Character.Movement;
using GameCore.Common;
using GameCore.Common.Messages;
using LocalMessages;
using UnityEngine;

namespace GameCore.Player
{
    public class TwoMousePlayer : MonoBehaviour, IPlayer
    {
        public PlayerMouseType MouseType { get; private set; }
        public CharacterMovement CurrentMovement { get; private set; }
        
        [Inject] private GameCamera _gameCamera;
        [Inject] private LocalMessageBroker _messageBroker;

        private RoundController _roundController;
        private CharacterMovement _fatMouseCharacter;
        private CharacterMovement _thinMouseCharacter;
        
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
            if (_roundController == null)
                return;
            
            if (!UnityEngine.Input.GetKeyDown(_roundController.settings.mouseChangeKey))
                return;
            
            if (_roundController.Stage != RoundStage.Game)
                return;
            
            PosessAnother();
                
            var message = new ChangeCharacterMessage();
            message.isThinMouse = MouseType == PlayerMouseType.ThinMouse;
            _messageBroker.Trigger(ref message);
        }

        public void Initialize(CharacterMovement fatMouse, CharacterMovement thinMouse)
        {
            _fatMouseCharacter = fatMouse;
            _thinMouseCharacter = thinMouse;
            
            _thinMouseCharacter.Unposess();
            PosessCharacter(_fatMouseCharacter);
        }

        public void PosessAnother() =>
            PosessCharacter(CurrentMovement == _fatMouseCharacter ? _thinMouseCharacter : _fatMouseCharacter);
        
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