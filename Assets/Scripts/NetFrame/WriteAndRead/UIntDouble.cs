using System.Runtime.InteropServices;

namespace NetFrame.WriteAndRead
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntDouble
    {
        [FieldOffset(0)]
        public double doubleValue;
        [FieldOffset(0)]
        public ulong longValue;
    }
}

