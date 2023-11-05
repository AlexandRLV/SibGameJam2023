namespace GameCore.Character.Interaction
{
    public class Trap : BaseTriggerObject
    {
        protected override void OnCharacterEnter()
        {
            // TODO: catch player
            enteredMovement.MoveValues.IsKnockdown = true;
        }
    }
}