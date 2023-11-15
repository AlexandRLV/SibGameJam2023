using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct PlayerJoinedRoomDataframe : INetworkDataframe
    {
        public int playerId;
        public string playerName;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteInt(playerId);
            writer.WriteString(playerName);
        }

        public void Read(NetFrameReader reader)
        {
            playerId = reader.ReadInt();
            playerName = reader.ReadString();
        }
    }
}