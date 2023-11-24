
namespace GameCore.InteractiveObjects
{
    public class GasDamage : BaseTriggerObject
    {
        protected override void OnPlayerEnter()
        {
            //SoundService.PlayRandomSound(SoundType.Mousetrap1, SoundType.Mousetrap2, SoundType.Mousetrap3);
            Movement.MoveValues.IsHit = true;
            Movement.Damage();
        }
    }
}

