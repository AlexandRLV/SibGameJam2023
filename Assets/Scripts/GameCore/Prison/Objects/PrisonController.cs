using System.Collections;
using Common;
using GameCore.Character.Animation;
using GameCore.Common;
using GameCore.InteractiveObjects;
using GameCore.RoundMissions.LocalMessages;
using GameCore.Sounds;
using LocalMessages;
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
            if (_isOpened) return;
            
            StartCoroutine(OpenDoorCoroutine());

            var message = new AgentSavedMessage();
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        }

        private IEnumerator OpenDoorCoroutine()
        {
            _isOpened = true;
            while (_openingTime < timeToOpen)
            {
                
                _openingTime += Time.deltaTime;
                door.eulerAngles = Vector3.Lerp(door.eulerAngles, openRot, Time.deltaTime * smooth);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            foreach (PrisonMouseController controller in mouseControllers)
            {
                controller.isReleased = true;
            }
        }

        public override void Interact()
        {
            if (IsUsed) return;
            IsUsed = true;
            OpenDoor();
        }
        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
            if (IsSeen) return;
            IsSeen = true;
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