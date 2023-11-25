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
        }
        
        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.CurrentInteractiveObject = this;
            _windowsSystem.TryGetWindow(out InGameUI inGameUI);
            inGameUI.InteractIndicatorState = true;
        }

        protected override void OnPlayerExit()
        {
            if (Movement.MoveValues.CurrentInteractiveObject != this)
                return;
            
            Movement.MoveValues.CurrentInteractiveObject = null;
            _windowsSystem.TryGetWindow(out InGameUI inGameUI);
            inGameUI.InteractIndicatorState = false;
        }

        protected virtual void OnInitialize() { }
        
        public abstract void Interact();
        public abstract void InteractWithoutPlayer(Vector3 playerPosition);
    }
}