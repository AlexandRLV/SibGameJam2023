using Common.DI;
using GameCore.Character.Animation;
using GameCore.LevelObjects.Abstract;
using GameCore.Player;
using GameCore.Sounds;
using UnityEngine;

namespace GameCore.LevelObjects.InteractiveObjects
{
    public class Cheese : InteractiveObject
    {
        public override AnimationType InteractAnimation => AnimationType.Eat;
        public override InteractiveObjectType Type => InteractiveObjectType.Cheese;
        public override Vector3 CheckPosition => transform.position;
        
        [SerializeField] private float speedMultiplier;
        [SerializeField] private float speedMultiplierDuration;

        [Inject] private IPlayer _player;

        public override void Interact()
        {
            soundService.PlaySound(SoundType.Eating);
            Movement.ChangeMovementSpeed(speedMultiplier, speedMultiplierDuration);
            Destroy(gameObject);
        }

        public override void InteractWithoutPlayer(Vector3 playerPosition)
        {
            soundService.PlaySound(SoundType.Eating);
            Destroy(gameObject);
        }

        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
            Movement.MoveValues.ForceInteract = true;
            
            if (IsSeen) return;
            IsSeen = true;
            
            soundService.PlaySound(_player.MouseType == PlayerMouseType.ThinMouse ? SoundType.ThinCheese : SoundType.FatCheese);
        }

        protected override void OnPlayerExit()
        {
            Movement.MoveValues.CurrentInteractiveObject = null;
        }
    }
}