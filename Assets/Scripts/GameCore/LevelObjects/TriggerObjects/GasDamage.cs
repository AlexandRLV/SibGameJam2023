
using GameCore.LevelObjects.Abstract;

namespace GameCore.LevelObjects.TriggerObjects
{
    public class GasDamage : BaseTriggerObject
    {
        protected override void OnPlayerEnter()
        {
            Movement.MoveValues.IsHit = true;
            Movement.Damage();
        }
    }
}

