using System.Collections;
using Common;
using GameCore.Character.Animation;
using GameCore.Common;
using GameCore.InteractiveObjects;
using GameCore.Player;
using GameCore.RoundMissions.LocalMessages;
using GameCore.Sounds;
using LocalMessages;
using UnityEngine;

namespace GameCore.Prison.Objects
{
    public class PrisonController : InteractiveObject
    {
        public override AnimationType InteractAnimation => AnimationType.OpenDoor;
        
        [SerializeField] private float doorOpenAngle = 90f;
        [SerializeField] private float timeToOpen = 2f;
        [SerializeField] private Transform door;
        [SerializeField] private PrisonMouseController[] mouseControllers;
        
        private float smooth = 2.0f;
        private float _openingTime;
        private bool _isOpened;
        private Vector3 defaultRot, openRot;
        
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
            
            var player = GameContainer.InGame.Resolve<GamePlayer>();
            SoundService.PlaySound(player.MouseType == PlayerMouseType.ThinMouse ? SoundType.ThinHostage : SoundType.FatHostage);
        }
    }
}