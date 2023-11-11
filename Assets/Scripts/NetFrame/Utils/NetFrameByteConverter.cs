using System;

namespace NetFrame.Utils
{
    public class NetFrameByteConverter
    {
        public byte[] GetByteArrayFromUInt(uint number)
        {
            byte[] byteArray = BitConverter.GetBytes(number);
        
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArray);
            }
        
            return byteArray;
        }
    
        public int GetUIntFromByteArray(byte[] byteArray)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArray);
            }
        
            return BitConverter.ToInt32(byteArray, 0);
        }
    }
}

