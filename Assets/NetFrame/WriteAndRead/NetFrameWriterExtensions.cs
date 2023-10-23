using System;
using System.Text;

namespace NetFrame.WriteAndRead
{
    public static class NetFrameWriterExtensions
    {
        private static readonly UTF8Encoding encoding = new UTF8Encoding(false, true);

        public static void WriteString(this NetFrameWriter writer, string value)
        {
            if (value == null)
            {
                writer.WriteUShort((ushort) 0);
            }
            else
            {
                int byteCount = NetFrameWriterExtensions.encoding.GetByteCount(value);
                writer.EnsureCapacity(writer.position + 2 + byteCount);
                writer.WriteUShort((ushort) byteCount);
                NetFrameWriterExtensions.encoding.GetBytes(value, 0, value.Length, writer.buffer, writer.position);
                writer.position += byteCount;
            }
        }

        public static void WriteSByte(this NetFrameWriter writer, sbyte value) => writer.WriteByte((byte) value);

        public static void WriteChar(this NetFrameWriter writer, char value) => writer.WriteUShort((ushort) value);

        public static void WriteBool(this NetFrameWriter writer, bool value) => writer.WriteByte(value ? (byte) 1 : (byte) 0);

        public static void WriteBytesAndSize(
            this NetFrameWriter writer,
            byte[] buffer,
            int offset,
            int count)
        {
            if (buffer == null)
            {
                writer.WriteUInt(0U);
            }
            else
            {
                writer.WriteUInt(checked ((uint) count) + 1U);
                writer.WriteBytes(buffer, offset, count);
            }
        }

        public static void WriteBytesAndSize(this NetFrameWriter writer, byte[] buffer) => writer.WriteBytesAndSize(buffer, 0, buffer != null ? buffer.Length : 0);

        public static void WriteSpanAndSize(this NetFrameWriter writer, ReadOnlySpan<byte> buffer)
        {
            writer.WriteUInt(checked ((uint) buffer.Length) + 1U);
            writer.WriteSpan(buffer);
        }

        public static void WriteGuid(this NetFrameWriter writer, Guid value)
        {
            byte[] byteArray = value.ToByteArray();
            writer.WriteBytes(byteArray, 0, byteArray.Length);
        }

        public static void WriteUri(this NetFrameWriter writer, Uri uri) => writer.WriteString(uri?.ToString());
    }
}