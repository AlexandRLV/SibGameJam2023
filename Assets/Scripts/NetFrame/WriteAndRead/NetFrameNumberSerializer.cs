using System;

namespace NetFrame.WriteAndRead
{
    public static class NetFrameNumberSerializer
    {
        public static short ReadShort(this NetFrameReader reader) => (short)reader.ReadUShort();

        public static ushort ReadUShort(this NetFrameReader reader) =>
            (ushort)((uint)(ushort)(0U | (uint)reader.ReadByte()) | (uint)(ushort)((uint)reader.ReadByte() << 8));

        public static int ReadInt(this NetFrameReader reader) => (int)reader.ReadUInt();

        public static uint ReadUInt(this NetFrameReader reader) => (uint)(0 | (int)reader.ReadByte() |
                                                                          (int)reader.ReadByte() << 8 |
                                                                          (int)reader.ReadByte() << 16 |
                                                                          (int)reader.ReadByte() << 24);

        public static long ReadLong(this NetFrameReader reader) => (long)reader.ReadULong();

        public static ulong ReadULong(this NetFrameReader reader) => (ulong)(0L | (long)reader.ReadByte() |
                                                                             (long)reader.ReadByte() << 8 |
                                                                             (long)reader.ReadByte() << 16 |
                                                                             (long)reader.ReadByte() << 24 |
                                                                             (long)reader.ReadByte() << 32 |
                                                                             (long)reader.ReadByte() << 40 |
                                                                             (long)reader.ReadByte() << 48 |
                                                                             (long)reader.ReadByte() << 56);

        public static float ReadFloat(this NetFrameReader reader) => new UIntFloat()
        {
            intValue = reader.ReadUInt()
        }.floatValue;

        public static double ReadDouble(this NetFrameReader reader) => new UIntDouble()
        {
            longValue = reader.ReadULong()
        }.doubleValue;

        public static Decimal ReadDecimal(this NetFrameReader reader) => new UIntDecimal()
        {
            longValue1 = reader.ReadULong(),
            longValue2 = reader.ReadULong()
        }.decimalValue;

        public static void WriteUShort(this NetFrameWriter writer, ushort value)
        {
            writer.WriteByte((byte)value);
            writer.WriteByte((byte)((uint)value >> 8));
        }

        public static void WriteShort(this NetFrameWriter writer, short value) => writer.WriteUShort((ushort)value);

        public static void WriteUInt(this NetFrameWriter writer, uint value)
        {
            writer.WriteByte((byte)value);
            writer.WriteByte((byte)(value >> 8));
            writer.WriteByte((byte)(value >> 16));
            writer.WriteByte((byte)(value >> 24));
        }

        public static void WriteInt(this NetFrameWriter writer, int value) => writer.WriteUInt((uint)value);

        public static void WriteULong(this NetFrameWriter writer, ulong value)
        {
            writer.WriteByte((byte)value);
            writer.WriteByte((byte)(value >> 8));
            writer.WriteByte((byte)(value >> 16));
            writer.WriteByte((byte)(value >> 24));
            writer.WriteByte((byte)(value >> 32));
            writer.WriteByte((byte)(value >> 40));
            writer.WriteByte((byte)(value >> 48));
            writer.WriteByte((byte)(value >> 56));
        }

        public static void WriteLong(this NetFrameWriter writer, long value) => writer.WriteULong((ulong)value);

        public static void WriteFloat(this NetFrameWriter writer, float value)
        {
            UIntFloat uintFloat = new UIntFloat()
            {
                floatValue = value
            };
            writer.WriteUInt(uintFloat.intValue);
        }

        public static void WriteDouble(this NetFrameWriter writer, double value)
        {
            UIntDouble uintDouble = new UIntDouble()
            {
                doubleValue = value
            };
            writer.WriteULong(uintDouble.longValue);
        }

        public static void WriteDecimal(this NetFrameWriter writer, Decimal value)
        {
            UIntDecimal uintDecimal = new UIntDecimal()
            {
                decimalValue = value
            };
            writer.WriteULong(uintDecimal.longValue1);
            writer.WriteULong(uintDecimal.longValue2);
        }
    }
}