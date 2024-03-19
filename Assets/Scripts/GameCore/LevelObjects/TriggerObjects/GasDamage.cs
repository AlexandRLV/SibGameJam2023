
using GameCore.LevelObjects.Abstract;

namespace GameCore.LevelObjects.TriggerObjects
{
    public class GasDamage : BaseTriggerObject
    {
        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.isHit = true;
            Movement.Damage();
        }
    }
}

