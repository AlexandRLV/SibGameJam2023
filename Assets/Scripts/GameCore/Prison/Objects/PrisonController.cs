using System.Collections;
using GameCore.Character.Animation;
using GameCore.InteractiveObjects;
using UnityEngine;

namespace GameCore.Prison.Objects
{
    public class PrisonController : InteractiveObject
    {
        [SerializeField] float openSpeed = 10;
        [SerializeField] float timeToOpen = 1;
        [SerializeField] Vector3 openingDirection = Vector3.down;
        [SerializeField] Transform door;
        float _openingTime;
        bool _isOpened;
        public override AnimationType InteractAnimation { get; } = AnimationType.OpenDoor;

        [SerializeField] PrisonMouseController[] mouseControllers;

        public PrisonController(AnimationType interactAnimation)
        {
            InteractAnimation = interactAnimation;
        }

        private void Awake()
        {
            mouseControllers = GetComponentsInChildren<PrisonMouseController>();
        }

        private void OpenDoor()
        {
            if (door == null || mouseControllers.Length == 0) return;
            if (!_isOpened) StartCoroutine(OpenDoorCoroutine());
        }

        private IEnumerator OpenDoorCoroutine()
        {
            _isOpened = true;
            while (_openingTime < timeToOpen)
            {
                _openingTime += Time.deltaTime;
                door.transform.Translate(openingDirection * (openSpeed * Time.deltaTime));
                yield return new WaitForSeconds(Time.deltaTime);
            }

            foreach (PrisonMouseController controller in mouseControllers)
            {
                controller.isReleased = true;
            }
        }

        public override void Interact()
        {
            OpenDoor();
        }
    }
}