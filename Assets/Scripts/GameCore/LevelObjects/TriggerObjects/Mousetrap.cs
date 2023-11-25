using Common.DI;
using GameCore.LevelObjects.Abstract;
using GameCore.Sounds;
using UnityEngine;

namespace GameCore.LevelObjects.TriggerObjects
{
    public class Mousetrap : BaseTriggerObject, ICheckPositionObject
    {
        public Vector3 CheckPosition => transform.position;
        
        [SerializeField] private GameObject cheese;

        [Inject] private LevelObjectService _levelObjectService;

        private void Start()
        {
            _levelObjectService.RegisterMousetrap(this);
        }

        private void OnDestroy()
        {
            _levelObjectService.UnregisterMousetrap(this);
        }

        public void Activate()
        {
            if (IsUsed) return;
            
            soundService.PlayRandomSound(SoundType.Mousetrap1, SoundType.Mousetrap2, SoundType.Mousetrap3);
            Destroy(cheese);
            
            IsUsed = true;
        }

        protected override void OnPlayerEnter()
        {
            if (IsUsed) return;
            
            soundService.PlayRandomSound(SoundType.Mousetrap1, SoundType.Mousetrap2, SoundType.Mousetrap3);
            Destroy(cheese);
            
            Movement.MoveValues.IsHit = true;
            Movement.Damage();
            
            IsUsed = true;
        }
    }
}