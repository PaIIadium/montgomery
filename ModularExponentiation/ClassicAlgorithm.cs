namespace ModularExponentiation
{
    using System.Collections.Generic;
    using System.Linq;
    using BO = BinaryOperations;

    public class ClassicAlgorithm
    {
        private static readonly List<bool> BinOne = new() {true};
        private static (int squareCount, List<bool> result) Cache = (0, BinOne);

        public static List<bool> ModularExponentiation(List<bool> a, List<bool> b, List<bool> modulo)
        {
            var result = BinOne.ToList();
            
            for (var i = b.Count - 1; i >= 0; i--)
            {
                if (b[i])
                {
                    var bitIndex = b.Count - i - 1;
                    result = GetRemainder(BO.Multiply(result, ModuloSquareNTime(a, bitIndex, modulo)), modulo);
                }
            }

            Cache = (0, BinOne);
            return result;
        }

        private static List<bool> ModuloSquareNTime(List<bool> a, int n, List<bool> modulo)
        {
            var result = Cache.squareCount != 0 ? Cache.result : a;
            for (var i = 0; i < n - Cache.squareCount; i++)
            {
                result = GetRemainder(BO.Multiply(result, result), modulo);
            }

            Cache = (n, result);
            return result;
        }

        private static List<bool> GetRemainder(List<bool> number, List<bool> modulo)
        {
            return BO.Divide(number, modulo).Remainder;
        }
    }
}