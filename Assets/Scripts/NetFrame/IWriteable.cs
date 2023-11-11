using NetFrame.WriteAndRead;

namespace NetFrame
{
    public interface IWriteable
    {
        void Write(NetFrameWriter writer);
    }
}

