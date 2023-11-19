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
        private enum OpenType
        {
            Angle,
            Move,
        }

        private enum OpenAxis
        {
            X, Y, Z,
        }
        
        public override AnimationType InteractAnimation => AnimationType.OpenDoor;

        [SerializeField] private OpenType _openType;
        [SerializeField] private OpenAxis _openAxis;
        [SerializeField] private float doorOpenAngle = 90f;
        [SerializeField] private float doorOpenDistance = 3f;
        [SerializeField] private float timeToOpen = 2f;
        [SerializeField] private Transform door;
        [SerializeField] private PrisonMouseController[] mouseControllers;
        
        private float smooth = 2.0f;
        private bool _isOpened;
        
        private void Awake()
        {
            mouseControllers = GetComponentsInChildren<PrisonMouseController>();
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
            if (_openType == OpenType.Angle)
            {
                var targetEuler = door.eulerAngles;
                targetEuler += _openAxis switch
                {
                    OpenAxis.X => new Vector3(doorOpenAngle, 0f, 0f),
                    OpenAxis.Y => new Vector3(0f, doorOpenAngle, 0f),
                    OpenAxis.Z => new Vector3(0f, 0f, doorOpenAngle),
                };
                var targetRotation = Quaternion.Euler(targetEuler);
                var originRotation = door.rotation;

                float timer = 0f;
                while (timer < timeToOpen)
                {
                    timer += Time.deltaTime;
                    float t = timer / timeToOpen;
                    door.rotation = Quaternion.Slerp(originRotation, targetRotation, t);
                    yield return null;
                }
            }
            else if (_openType == OpenType.Move)
            {
                var targetPosition = door.position;
                targetPosition += _openAxis switch
                {
                    OpenAxis.X => Vector3.right * doorOpenDistance,
                    OpenAxis.Y => Vector3.up * doorOpenDistance,
                    OpenAxis.Z => Vector3.forward * doorOpenDistance
                };
                var originPosition = door.position;
                
                float timer = 0f;
                while (timer < timeToOpen)
                {
                    timer += Time.deltaTime;
                    float t = timer / timeToOpen;
                    door.position = Vector3.Lerp(originPosition, targetPosition, t);
                    yield return null;
                }
            }

            foreach (var controller in mouseControllers)
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