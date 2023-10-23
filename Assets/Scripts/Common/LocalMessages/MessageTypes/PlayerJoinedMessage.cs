namespace SibGameJam.Common.LocalMessages.MessageTypes
{
    public struct PlayerJoinedMessage : ILocalMessage
    {
        public int Id;
        public string Name;
    }
}