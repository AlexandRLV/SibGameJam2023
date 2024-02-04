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

        [SerializeField] private GameObject _interactIndicator;
        
        [Inject] private LevelObjectService _levelObjectService;
        [Inject] private WindowsSystem _windowsSystem;

        private void Start()
        {
            GameContainer.InjectToInstance(this);
            _levelObjectService.RegisterInteractiveObject(this);
            _interactIndicator.SetActive(false);
            OnInitialize();
        }

        private void OnDestroy()
        {
            _levelObjectService?.UnregisterInteractiveObject(this);
            OnPlayerExit();
        }
        
        public void Interact()
        {
            if (IsUsed)
                return;

            IsUsed = true;
            OnInteractInternal();
            SetInteractIndicatorState(false);
        }

        public void SetInteractIndicatorState(bool state)
        {
            if (IsUsed) state = false;
            _interactIndicator.SetActive(state);
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
        
        protected abstract void OnInteractInternal();
        public abstract void InteractWithoutPlayer(Vector3 playerPosition);
    }
}