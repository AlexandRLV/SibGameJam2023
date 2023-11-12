using NetFrame;
using NetFrame.WriteAndRead;

namespace Networking.Dataframes
{
    public struct CreateRoomDataframe : INetworkDataframe
    {
        public string name;
        public string password;
    
        public void Write(NetFrameWriter writer)
        {
            writer.WriteString(name);
            writer.WriteString(password);
        }

        public void Read(NetFrameReader reader)
        {
            name = reader.ReadString();
            password = reader.ReadString();
        }
    }
}