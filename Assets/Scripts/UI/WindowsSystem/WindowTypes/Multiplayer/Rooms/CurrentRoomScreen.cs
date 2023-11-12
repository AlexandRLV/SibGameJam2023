using Networking.Dataframes;

namespace UI.WindowsSystem.WindowTypes.Multiplayer.Rooms
{
    public class CurrentRoomScreen : WindowBase
    {
        private RoomInfoDataframe _room;

        public void Setup(RoomInfoDataframe room)
        {
            _room = room;
            // TODO: setup
        }
    }
}