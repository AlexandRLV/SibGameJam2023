using System.Collections;
using Common;
using GameCore.Character.Animation;
using GameCore.Common;
using GameCore.InteractiveObjects;
using GameCore.Sounds;
using UnityEngine;

namespace GameCore.Prison.Objects
{
    public class PrisonController : InteractiveObject
    {
        Vector3 defaultRot, openRot;
        float smooth = 2.0f;
        [SerializeField] float doorOpenAngle = 90f;
        [SerializeField] Transform door;
        float _openingTime;
        [SerializeField] float timeToOpen = 2f;
        bool _isOpened = false;
        public override AnimationType InteractAnimation => AnimationType.OpenDoor;

        [SerializeField] PrisonMouseController[] mouseControllers;
        private void Awake()
        {
            mouseControllers = GetComponentsInChildren<PrisonMouseController>();
            defaultRot = door.eulerAngles;
            openRot = new Vector3(defaultRot.x, defaultRot.y + doorOpenAngle, defaultRot.z);
        }

        private void OpenDoor()
        {
            if (door == null || mouseControllers.Length == 0) return;
            if (!_isOpened)
            {
                StartCoroutine(OpenDoorCoroutine());
                var roundController = GameContainer.InGame.Resolve<RoundController>();
                roundController.SaveMouse();
            }
        }

        private IEnumerator OpenDoorCoroutine()
        {
            _isOpened = true;
            while (_openingTime < timeToOpen)
            {
                
                _openingTime += Time.deltaTime;
                door.eulerAngles = Vector3.Slerp(door.eulerAngles, openRot, Time.deltaTime * smooth);
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
        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
            switch (RoundController.Stage)
            {
                case RoundStage.ThinMouse:
                    SoundService.PlaySound(SoundType.ThinHostage);
                    break;
                case RoundStage.FatMouse:
                    SoundService.PlaySound(SoundType.FatHostage);
                    break;
            }
        }
    }
}