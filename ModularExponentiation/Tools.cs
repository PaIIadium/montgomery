namespace ModularExponentiation
{
    public static class Tools
    {
        private const uint Mask = 0x80000000;
        public static bool GetBit(uint value, int bitNumber)
        {
            return (value & (Mask >> bitNumber)) != 0;
        }
        
        public static uint SetBitAt(uint num, int bitPos)
        {
            return num | (uint)(1 << (31 - bitPos));
        }
    }
}