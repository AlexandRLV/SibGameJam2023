using GameCore.Sounds;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class Mousetrap : BaseTriggerObject
    {
        [SerializeField] private GameObject cheese;

        protected override void OnPlayerEnter()
        {
            if (IsUsed) return;
            
            SoundService.PlayRandomSound(SoundType.Mousetrap1, SoundType.Mousetrap2, SoundType.Mousetrap3);
            Destroy(cheese);
            Movement.MoveValues.IsKnockdown = true;
            Movement.Damage();
            IsUsed = true;
        }

        protected override void OnPlayerStay()
        {
        }

        protected override void OnPlayerExit()
        {
        }
    }
}