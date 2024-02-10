namespace Startup.GameStateMachine
{
    public interface IGameState
    {
        public void OnEnter();
        public void OnExit();
    }
}