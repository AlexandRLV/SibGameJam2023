using System;
using System.Text;

namespace NetFrame.WriteAndRead
{
    public static class NetFrameReaderExtensions
    {
        private static readonly UTF8Encoding encoding = new UTF8Encoding(false, true);

        public static byte ReadByte(this NetFrameReader reader) => reader.ReadByte();

        public static sbyte ReadSByte(this NetFrameReader reader) => (sbyte) reader.ReadByte();

        public static char ReadChar(this NetFrameReader reader) => (char) reader.ReadUShort();

        public static bool ReadBool(this NetFrameReader reader) => reader.ReadByte() > (byte) 0;

        public static string ReadString(this NetFrameReader reader)
        {
            ushort count = reader.ReadUShort();
            if (count == (ushort) 0)
                return (string) null;
            ReadOnlySpan<byte> bytes = reader.ReadSpan((int) count);
            return NetFrameReaderExtensions.encoding.GetString(bytes);
        }

        public static byte[] ReadBytesAndSize(this NetFrameReader reader)
        {
            uint num = reader.ReadUInt();
            return num != 0U ? reader.ReadBytes(checked ((int) (num - 1U))) : (byte[]) null;
        }

        public static ReadOnlySpan<byte> ReadSpanAndSize(this NetFrameReader reader)
        {
            int count = checked ((int) (reader.ReadUInt() - 1U));
            return reader.ReadSpan(count);
        }

        public static byte[] ReadBytes(this NetFrameReader reader, int count)
        {
            byte[] bytes = new byte[count];
            reader.ReadBytes(bytes, count);
            return bytes;
        }

        public static Guid ReadGuid(this NetFrameReader reader) => new Guid(reader.ReadSpan(16));

        public static Uri ReadUri(this NetFrameReader reader)
        {
            string uriString = reader.ReadString();
            return !string.IsNullOrEmpty(uriString) ? new Uri(uriString) : (Uri) null;
        }
    }
}

