using Common;
using GameCore.Character.Animation;
using GameCore.LevelObjects;

namespace GameCore.InteractiveObjects
{
    public abstract class InteractiveObject : BaseTriggerObject
    {
        public abstract AnimationType InteractAnimation { get; }
        public abstract InteractiveObjectType Type { get; }
        
        public abstract void Interact();
        public abstract void InteractWithoutPlayer();

        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
        }

        protected override void OnPlayerExit()
        {
            if (Movement.MoveValues.CurrentInteractiveObject == this)
                Movement.MoveValues.CurrentInteractiveObject = null;
        }

        private void OnEnable()
        {
            GameContainer.InGame.Resolve<LevelObjectService>().RegisterInteractiveObject(this);
        }

        private void OnDisable()
        {
            if (GameContainer.InGame.CanResolve<LevelObjectService>())
                GameContainer.InGame.Resolve<LevelObjectService>().UnregisterInteractiveObject(this);
        }
    }
}