using GameCore.Character.Movement;

namespace GameCore.Player
{
    public interface IPlayer
    {
        public PlayerMouseType MouseType { get; }
        public CharacterMovement CurrentMovement { get; }

        public void Unposess();
    }
}