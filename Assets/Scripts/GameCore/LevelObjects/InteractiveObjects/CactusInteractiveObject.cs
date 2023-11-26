using System.Collections;
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
        private const float AnimateTime = 0.5f;
        
        public override AnimationType InteractAnimation => AnimationType.Eat;
        public override InteractiveObjectType Type => InteractiveObjectType.Cactus;
        public override Vector3 CheckPosition => transform.position;

        [Inject] private LocalMessageBroker _messageBroker;

        protected override void OnInitialize()
        {
            GameContainer.InjectToInstance(this);
        }

        public override void Interact()
        {
            Debug.Log("Interact with cactus");
            if (IsUsed)
            {
                Debug.Log("Already interacted");
                return;
            }

            IsUsed = true;
            Debug.Log("Set interacted to true, start animation");
            
            var message = new CactusFoundMessage();
            _messageBroker.Trigger(ref message);

            StartCoroutine(Animate(AnimateTime, Movement.gameObject.transform.position));

            OnPlayerExit();
        }

        public override void InteractWithoutPlayer(Vector3 playerPosition)
        {
            Debug.Log("Trigger cactus found message");
            IsUsed = true;
            
            var message = new CactusFoundMessage();
            _messageBroker.Trigger(ref message);
            
            StartCoroutine(Animate(AnimateTime, playerPosition));
        }

        private IEnumerator Animate(float time, Vector3 targetPosition)
        {
            float timer = 0f;
            var startPosition = transform.position;
            var startScale = transform.localScale;
            
            while (timer < time)
            {
                timer += Time.deltaTime;
                float t = timer / time;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                yield return null;
            }
            
            gameObject.SetActive(false);
        }
    }
}