using System.Collections;
using Common.DI;
using GameCore.Character.Visuals;
using GameCore.Player;
using GameCore.Prison.Objects;
using UnityEngine;

namespace GameCore.Prison.Mouse
{
    public class PrisonMouseController : MonoBehaviour
    {
        [SerializeField] private float _helpAnimationActivateDistance;
        [SerializeField] private PrisonController _prisonController;
        [SerializeField] private PrisonMouseMovement _movement;

        private bool _isReleased;
        private IPlayer _player;
    
        private IEnumerator Start()
        {
            _movement.Init();

            while (!GameContainer.InGame.CanResolve<IPlayer>())
            {
                yield return null;
            }

            _player = GameContainer.InGame.Resolve<IPlayer>();
            _prisonController.OnDoorStartOpen += OnDoorStartOpen;
            _prisonController.OnDoorOpen += OnDoorOpen;
        }

        private void FixedUpdate()
        {
            if (_isReleased)
                return;

            if (_player == null || _player.CurrentMovement == null)
            {
                _movement.CurrentAnimation = AnimationType.Walk;
                return;
            }
        
            float distanceToPlayer = Vector3.Distance(transform.position, _player.CurrentMovement.transform.position);
            _movement.CurrentAnimation = distanceToPlayer > _helpAnimationActivateDistance ? AnimationType.Walk : AnimationType.Push;
        }

        private void OnDoorStartOpen()
        {
            _isReleased = true;
            _movement.CurrentAnimation = AnimationType.Walk;
            _prisonController.OnDoorStartOpen -= OnDoorStartOpen;
        }

        private void OnDoorOpen()
        {
            _isReleased = true;
            _movement.CurrentAnimation = AnimationType.Walk;
            _movement.Evacuate();
        
            _prisonController.OnDoorOpen -= OnDoorOpen;
        }
    }
}