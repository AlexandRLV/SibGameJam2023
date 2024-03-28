using System.Collections.Generic;
using GameCore.Character.Movement;

namespace GameCore.Player
{
    public interface IPlayer
    {
        public PlayerMouseType MouseType { get; }
        public CharacterMovement CurrentMovement { get; }
        public CharacterMovement LastMovement { get; }
        public List<int> MovementObjects { get; }

        public void Unposess();
    }
}