using System;
using System.Runtime.CompilerServices;

namespace NetFrame.WriteAndRead
{
    public class NetFrameWriter
    {
        internal byte[] buffer;
        public int position;
    
        public void Reset()
        {
            position = 0;
        }
    
        public NetFrameWriter(int defaultCapacity)
        {
            buffer = new byte[defaultCapacity];
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void EnsureCapacity(int value)
        {
            if (buffer.Length < value)
            {
                int capacity = Math.Max(value, buffer.Length * 2);
                Array.Resize(ref buffer, capacity);
            }
        }
    
        public byte[] ToArray()
        {
            byte[] data = new byte[position];
            Array.ConstrainedCopy(buffer, 0, data, 0, position);
            return data;
        }
        
        public ArraySegment<byte> ToArraySegment()
        {
            return new ArraySegment<byte>(buffer, 0, position);
        }
    
        public void WriteByte(byte value)
        {
            EnsureCapacity(position + 1);
            buffer[position++] = value;
        }
    
        public void WriteBytes(byte[] bytes, int offset, int count)
        {
            EnsureCapacity(position + count);
            Array.ConstrainedCopy(bytes, offset, buffer, position, count);
            position += count;
        }
    
        public void WriteSpan(ReadOnlySpan<byte> data)
        {
            EnsureCapacity(position + data.Length);
            data.CopyTo(buffer.AsSpan(position, data.Length));
            position += data.Length;
        }
    
        public void WriteMemory(ReadOnlyMemory<byte> data)
        {
            EnsureCapacity(position + data.Length);
            data.CopyTo(buffer.AsMemory(position, data.Length));
            position += data.Length;
        }
        
        public void Write<T>(T value) where T : IWriteable => value.Write(this);
    }
}