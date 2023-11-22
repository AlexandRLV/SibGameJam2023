using Common;
using GameCore.Character.Animation;
using GameCore.Player;
using GameCore.Player.Network;
using GameCore.Sounds;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class Cheese : InteractiveObject
    {
        [SerializeField] private float speedMultiplier;
        [SerializeField] private float speedMultiplierDuration;

        public override AnimationType InteractAnimation => AnimationType.Eat;

        public override void Interact()
        {
            SoundService.PlaySound(SoundType.Eating);
            Movement.ChangeMovementSpeed(speedMultiplier, speedMultiplierDuration);
            Destroy(gameObject);
        }

        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
            Movement.MoveValues.ForceInteract = true;
            if (IsSeen) return;
            IsSeen = true;
            
            var player = GameContainer.InGame.Resolve<IPlayer>();
            SoundService.PlaySound(player.MouseType == PlayerMouseType.ThinMouse ? SoundType.ThinCheese : SoundType.FatCheese);
        }

        protected override void OnPlayerExit()
        {
            Movement.MoveValues.CurrentInteractiveObject = null;
        }
    }
}