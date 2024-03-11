using Cysharp.Threading.Tasks;

namespace Startup.GameStateMachine
{
    public interface IGameState
    {
        public UniTask OnEnter();
        public UniTask OnExit();
    }
}