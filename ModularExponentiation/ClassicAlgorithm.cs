namespace ModularExponentiation
{
    using System.Collections.Generic;
    using System.Linq;
    using BO = BinaryOperations;

    public class ClassicAlgorithm
    {
        private static readonly List<bool> BinOne = new() {true};
        private static (int squareCount, List<bool> result) Cache = (0, BinOne);

        public static List<bool> ModularExponentiation(List<bool> a, List<bool> b, List<bool> n)
        {
            var result = BinOne.ToList();
            for (var i = 0; i < b.Count; i++)
            {
                if (b[i])
                {
                    result = BO.Divide(BO.Multiply(result, SquareNTime(a, i)), n).Remainder;
                }
            }

            return result;
        }

        private static List<bool> SquareNTime(List<bool> a, int n)
        {
            var result = Cache.squareCount != 0 ? Cache.result : a;
            for (var i = 0; i < n - Cache.squareCount; i++)
            {
                result = BO.Multiply(result, result);
            }

            Cache = (n, result);
            return result;
        }
    }
}