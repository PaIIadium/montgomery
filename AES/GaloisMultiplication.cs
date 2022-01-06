namespace AES
{
    public static class GaloisMultiplication
    {
        private static readonly byte[,] Table = new byte[256,256];
        
        public static void Initialize()
        {
            for (var i = 0; i < 256; i++)
            {
                for (var j = 0; j < 256; j++)
                {
                    var result = GaloisMul((byte)i, (byte)j);
                    Table[i, j] = result;
                }
            }
        }

        public static byte Mul(byte a, byte b)
        {
            return Table[a, b];
        }

        private static byte GaloisMul(byte a, byte b) {
            byte p = 0;

            for (int counter = 0; counter < 8; counter++) {
                if ((b & 1) != 0) {
                    p ^= a;
                }

                bool hi_bit_set = (a & 0x80) != 0;
                a <<= 1;
                if (hi_bit_set)
                {
                    a ^= 0x1B;
                }
                b >>= 1;
            }

            return p;
        }
    }
}