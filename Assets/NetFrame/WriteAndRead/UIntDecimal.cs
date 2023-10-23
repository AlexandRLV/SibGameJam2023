using System;
using System.Runtime.InteropServices;

namespace NetFrame.WriteAndRead
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntDecimal
    {
        [FieldOffset(0)]
        public ulong longValue1;
        [FieldOffset(8)]
        public ulong longValue2;
        [FieldOffset(0)]
        public Decimal decimalValue;
    }
}