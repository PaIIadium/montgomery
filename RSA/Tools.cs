namespace RSA
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Tools
    {
        public static void PadWithZeros(List<bool> bits, int requiredSize)
        {
            var remainingBitsCount = requiredSize - bits.Count;
            if (remainingBitsCount != 0)
            {
                var additionalBits = new bool[remainingBitsCount];
                bits.InsertRange(0, additionalBits);
            }
        }
        
        public static List<bool> SkipFirstZeros(List<bool> bits)
        {
            var result = bits.SkipWhile(bit => !bit).ToList();
            if (result.Count == 0) result.Add(false);
            return result;
        }
    }
}