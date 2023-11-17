
namespace GameCore.InteractiveObjects
{
    public class GasDamage : BaseTriggerObject
    {
        protected override void OnPlayerEnter()
        {
            print("Hello");
            //SoundService.PlayRandomSound(SoundType.Mousetrap1, SoundType.Mousetrap2, SoundType.Mousetrap3);
            Movement.MoveValues.IsHit = true;
            Movement.Damage();
        }

        protected override void OnPlayerStay()
        {
        }

        protected override void OnPlayerExit()
        {
        }
    }
}

