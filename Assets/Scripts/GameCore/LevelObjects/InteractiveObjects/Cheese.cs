using Common.DI;
using GameCore.Character.Animation;
using GameCore.LevelAchievements.LocalMessages;
using GameCore.LevelObjects.Abstract;
using GameCore.LevelObjects.Messages;
using GameCore.Player;
using GameCore.Sounds;
using LocalMessages;
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

        protected override void OnInteractInternal()
        {
            var message = new CheeseCollectedMessage();
            localMessageBroker.Trigger(ref message);
            
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
            base.OnPlayerEnter();
            Movement.MoveValues.forceInteract = true;
            
            if (IsSeen) return;
            IsSeen = true;
            
            var player = GameContainer.InGame.Resolve<IPlayer>();
            soundService.PlaySound(player.MouseType == PlayerMouseType.ThinMouse ? SoundType.ThinCheese : SoundType.FatCheese);
        }
    }
}