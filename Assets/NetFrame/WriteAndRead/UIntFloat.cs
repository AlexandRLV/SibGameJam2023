using System.Runtime.InteropServices;

namespace NetFrame.WriteAndRead
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntFloat
    {
        [FieldOffset(0)]
        public float floatValue;
        [FieldOffset(0)]
        public uint intValue;
    }
}

