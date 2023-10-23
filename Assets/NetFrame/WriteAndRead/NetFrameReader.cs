using System;
using System.IO;

namespace NetFrame.WriteAndRead
{
    public class NetFrameReader
    {
        internal ArraySegment<byte> buffer;
        public int position;
        public int length => buffer.Count;

        public ReadOnlySpan<byte> Span => buffer.AsSpan();

        public int Remaining => length - position;

        public NetFrameReader(byte[] bytes)
        {
            buffer = new ArraySegment<byte>(bytes);
        }

        public NetFrameReader(ArraySegment<byte> segment)
        {
            buffer = segment;
        }

        public void SetBuffer(byte[] bytes)
        {
            buffer = new ArraySegment<byte>(bytes);
            position = 0;
        }

        public void SetBuffer(ArraySegment<byte> segment)
        {
            buffer = segment;
            position = 0;
        }

        public byte ReadByte()
        {
            if (position + 1 > buffer.Count)
            {
                throw new EndOfStreamException("ReadByte out of range:" + ToString());
            }

            return buffer.Array[buffer.Offset + position++];
        }


        public byte[] ReadBytes(byte[] bytes, int count)
        {
            if (count > bytes.Length)
            {
                throw new EndOfStreamException("ReadBytes can't read " + count +
                                               " + bytes because the passed byte[] only has length " + bytes.Length);
            }

            ArraySegment<byte> data = ReadBytesSegment(count);
            Array.Copy(data.Array, data.Offset, bytes, 0, count);
            return bytes;
        }


        public ArraySegment<byte> ReadBytesSegment(int count)
        {
            if (position + count > buffer.Count)
            {
                throw new EndOfStreamException("ReadBytesSegment can't read " + count +
                                               " bytes because it would read past the end of the stream. " +
                                               ToString());
            }


            ArraySegment<byte> result = new ArraySegment<byte>(buffer.Array, buffer.Offset + position, count);
            position += count;
            return result;
        }

        public ReadOnlySpan<byte> ReadSpan(int count)
        {
            var bytes = new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + position, count);
            position += count;
            return bytes;
        }

        public override string ToString()
        {
            return
                $"IBNet_Reader pos={position} len={length} buffer={BitConverter.ToString(buffer.Array, buffer.Offset, buffer.Count)}";
        }

        public T Read<T>() where T : struct, IReadable
        {
            var value = new T();
            value.Read(this);
            return value;
        }
    }
}