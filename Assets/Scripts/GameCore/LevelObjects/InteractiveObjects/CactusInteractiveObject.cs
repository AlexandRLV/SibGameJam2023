using Common.DI;
using GameCore.Character.Animation;
using GameCore.LevelObjects.Abstract;
using GameCore.RoundMissions.LocalMessages;
using LocalMessages;
using UnityEngine;

namespace GameCore.LevelObjects.InteractiveObjects
{
    public class CactusInteractiveObject : InteractiveObject
    {
        public override AnimationType InteractAnimation => AnimationType.Eat;
        public override InteractiveObjectType Type => InteractiveObjectType.Cactus;
        public override Vector3 CheckPosition => transform.position;

        [Inject] private LocalMessageBroker _messageBroker;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private Vector3 _startScale;
        private Vector3 _endScale;
        private bool _canStart;
        private bool _isFinished;

        protected override void OnInitialize()
        {
            GameContainer.InjectToInstance(this);
        }

        public override void Interact()
        {
            var message = new CactusFoundMessage();
            _messageBroker.Trigger(ref message);
        
            _startScale = transform.localScale;
            _endScale = Vector3.zero;
            
            _startPosition = transform.position;
            _targetPosition = Movement.gameObject.transform.position;
            
            _canStart = true;
            
            OnPlayerExit();
        }

        public override void InteractWithoutPlayer(Vector3 playerPosition)
        {
            Debug.Log("Trigger cactus found message");
            var message = new CactusFoundMessage();
            _messageBroker.Trigger(ref message);
        
            _startScale = transform.localScale;
            _endScale = Vector3.zero;
            
            _startPosition = transform.position;
            _targetPosition = playerPosition;
            
            _canStart = true;
        }

        private void Update()
        {
            if (!_canStart || _isFinished) return;
            if (Movement == null)
            {
                _isFinished = true;
                gameObject.SetActive(false);
                return;
            }

            transform.position = Vector3.Lerp(_startPosition, _targetPosition, Time.deltaTime * 2f);
            transform.localScale = Vector3.Lerp(_startScale, _endScale, Time.deltaTime * 20f);

            if (Vector3.Distance(_startPosition, _targetPosition) > 0.5f) return;

            _isFinished = true;
            gameObject.SetActive(false);
        }
    }
}