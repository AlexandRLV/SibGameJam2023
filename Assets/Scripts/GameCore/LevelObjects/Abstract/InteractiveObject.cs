using Common.DI;
using GameCore.Character.Animation;
using UnityEngine;

namespace GameCore.LevelObjects.Abstract
{
    public abstract class InteractiveObject : BaseTriggerObject, ICheckPositionObject
    {
        public abstract AnimationType InteractAnimation { get; }
        public abstract InteractiveObjectType Type { get; }
        public abstract Vector3 CheckPosition { get; }
        
        [Inject] private LevelObjectService _levelObjectService;
        
        public abstract void Interact();
        public abstract void InteractWithoutPlayer(Vector3 playerPosition);
        
        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
        }

        protected override void OnPlayerExit()
        {
            if (Movement.MoveValues.CurrentInteractiveObject == this)
                Movement.MoveValues.CurrentInteractiveObject = null;
        }

        private void Start()
        {
            _levelObjectService.RegisterInteractiveObject(this);
        }

        private void OnDestroy()
        {
            _levelObjectService?.UnregisterInteractiveObject(this);
        }
    }
}