using Common.DI;
using GameCore.Character.Animation;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;

namespace GameCore.LevelObjects.Abstract
{
    public abstract class InteractiveObject : BaseTriggerObject, ICheckPositionObject
    {
        public abstract AnimationType InteractAnimation { get; }
        public abstract InteractiveObjectType Type { get; }
        public abstract Vector3 CheckPosition { get; }
        
        [Inject] private LevelObjectService _levelObjectService;
        [Inject] private WindowsSystem _windowsSystem;

        private void Start()
        {
            GameContainer.InjectToInstance(this);
            _levelObjectService.RegisterInteractiveObject(this);
            OnInitialize();
        }

        private void OnDestroy()
        {
            _levelObjectService?.UnregisterInteractiveObject(this);
            OnPlayerExit();
        }
        
        protected override void OnPlayerEnter()
        {
            Movement.SetCurrentInteractiveObject(this);
        }

        protected override void OnPlayerExit()
        {
            if (Movement != null && Movement.MoveValues.CurrentInteractiveObject == this)
                Movement.SetCurrentInteractiveObject(null);
        }

        protected virtual void OnInitialize() { }
        
        public abstract void Interact();
        public abstract void InteractWithoutPlayer(Vector3 playerPosition);
    }
}