using Common.DI;
using GameCore.Camera;
using GameCore.Character.Movement;
using UnityEngine;

namespace GameCore.Player
{
    public class OneMouseSinglePlayer : MonoBehaviour, IPlayer
    {
        public PlayerMouseType MouseType { get; private set; }
        public CharacterMovement CurrentMovement { get; private set; }
        
        [Inject] private GameCamera _gameCamera;

        public void Initialize(CharacterMovement movement, PlayerMouseType mouseType)
        {
            CurrentMovement = movement;
            CurrentMovement.Posess();

            MouseType = mouseType;
            
            _gameCamera.SetTarget(movement.transform);
        }
        
        public void Unposess()
        {
            if (CurrentMovement != null)
                CurrentMovement.Unposess();

            CurrentMovement = null;
        }
    }
}