namespace ModularExponentiation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BO = BinaryOperations;
    using DO = DecimalOperations;
    
    class Program
    {
        private static readonly Dictionary<int, string> BinaryPowers = new();
        private static readonly Dictionary<int, int> BinaryPowersLengths = new();
        private static readonly double Log2_10 = Math.Log2(10);
        private const int MaxBinaryPower = 2048;

        private static void Main(string[] args)
        {
            var a = args[0];
            var b = args[1];
            var n = args[2];
            FillBinaryPowers();
            var aBin = ConvertDecimalToBinary(a);
            var bBin = ConvertDecimalToBinary(b);
            var nBin = ConvertDecimalToBinary(n);

            var product = MontgomeryMultiply(aBin, bBin, nBin);
            var decimalProduct = ConvertBinaryToDecimal(product);
            Console.WriteLine($"{a} * {b} (mod {n}) = {decimalProduct}");
            var modExp = ModularExponentiation(aBin, bBin, nBin);
            var decimalModExp = ConvertBinaryToDecimal(modExp);
            Console.WriteLine($"{a} ^ {b} (mod {n}) = {decimalModExp}");
        }

        private static void FillBinaryPowers()
        {
            var value = "1";
            BinaryPowers[0] = value;
            for (var i = 1; i <= MaxBinaryPower; i++)
            {
                value = MultiplyByTwo(value);
                BinaryPowers[i] = value;
                if (!BinaryPowersLengths.ContainsKey(value.Length))
                {
                    BinaryPowersLengths[value.Length] = i;
                }
            }
        }

        private static string MultiplyByTwo(string number)
        {
            var result = new char[number.Length + 1];
            var overflow = false;
            for (var i = number.Length - 1; i >= 0; i--)
            {
                var digit = number[i];
                var byteDigit = Dictionaries.CharToByte[digit];
                var resultDigit = (byte) (byteDigit * 2);
                if (overflow) resultDigit += 1;
                if (resultDigit >= 10)
                {
                    resultDigit -= 10;
                    overflow = true;
                }
                else
                {
                    overflow = false;
                }
                result[i + 1] = Dictionaries.ByteToChar[resultDigit];
            }

            if (overflow)
            {
                result[0] = '1';
                return new string (result);
            }

            return new string(result[1..]);
        }
        
        private static List<bool> ConvertDecimalToBinary(string number)
        {
            var result = new bool[(int)Math.Ceiling(number.Length * Log2_10)];
            var remainder = number;
            while (remainder != "0")
            {
                var binaryPowerIndex = FindFirstLargerBinaryPowerIndex(remainder) - 1;
                result[binaryPowerIndex] = true;
                remainder = DO.Subtract(remainder, BinaryPowers[binaryPowerIndex]);
            }

            return result.Reverse().SkipWhile(bit => !bit).ToList();
        }

        private static int FindFirstLargerBinaryPowerIndex(string number)
        {
            var binaryPowerIndex = BinaryPowersLengths[number.Length];
            while (true)
            {
                var comparingResult = DO.Compare(BinaryPowers[binaryPowerIndex], number);
                if (comparingResult == 1) return binaryPowerIndex;
                binaryPowerIndex++;
            }
        }
        
        private static List<bool> MontgomeryMultiply(List<bool> a, List<bool> b, List<bool> n)
        {
            var rIndex = n.Count;
            var aResidue = FindNResidue(a, rIndex, n);
            var bResidue = FindNResidue(b, rIndex, n);
            var t = BO.Multiply(aResidue, bResidue);
            
            var result = Calculate(t, rIndex, n);
            return Calculate(result, rIndex, n);
        }

        private static List<bool> FindNResidue(List<bool> number, int rIndex, List<bool> modulo)
        {
            var product = number.ToList();
            product.AddRange(new bool[rIndex]);
            return BO.Divide(product, modulo).Remainder;
        }

        private static List<bool> Calculate(List<bool> t, int rIndex, List<bool> n)
        {
            var rBinaryPower = GetBinaryPower(rIndex);
            var inversedN = Inverse(n, rBinaryPower);
            
            var minusInversedN = BO.Subtract(rBinaryPower, inversedN);
            var k = FindRemainder(BO.Multiply(t, minusInversedN), rIndex);
            var result = BO.DivideByBinaryPower(BO.Add(t, BO.Multiply(k, n)), rIndex);

            if (BO.Compare(result, n) is 0 or 1) result = BO.Subtract(result, n);
            return result;
        }

        private static List<bool> FindRemainder(List<bool> binaryNumber, int binaryPowerIndex)
        {
            return binaryNumber
                .GetRange(binaryNumber.Count - binaryPowerIndex, binaryPowerIndex)
                .SkipWhile(bit => !bit).ToList();
        }

        private static List<bool> Inverse(List<bool> a, List<bool> b)
        {
            var modulo = b.ToList();
            var x = new List<bool> {false};
            var x2 = new List<bool> {true};
            var x1 = new List<bool> {false};
            
            while (!(b.Count == 1 && !b[0])) {

                var divisionResult = BO.Divide(a, b);
                var q = divisionResult.Quotient;
                var r = divisionResult.Remainder;
                x = BO.Subtract(x2, BO.Multiply(q, x1), modulo);
                a = b;
                b = r;
                x2 = x1;
                x1 = x;
            }

            return x2;
        }

        private static List<bool> GetBinaryPower(int binaryPowerIndex)
        {
            var binaryPower = new bool[binaryPowerIndex + 1];
            binaryPower[0] = true;
            return new List<bool>(binaryPower);
        }
        
        private static string ConvertBinaryToDecimal(List<bool> binary)
        {
            var result = "0";
            for (var i = 0; i < binary.Count; i++)
            {
                var index = binary.Count - 1 - i;
                if (binary[index])
                {
                    var binaryPower = BinaryPowers[i];
                    result = DO.Add(result, binaryPower);
                }
            }

            return result;
        }

        private static List<bool> ModularExponentiation(List<bool> number, List<bool> exponent, List<bool> modulo)
        {
            var rIndex = modulo.Count;
            var aResidue = FindNResidue(number, rIndex, modulo);
            var x = FindNResidue(new List<bool> {true}, rIndex, modulo);
            foreach (var bit in exponent)
            {
                var xSqr = BO.Multiply(x, x);
                x = Calculate(xSqr, rIndex, modulo);
                if (bit)
                {
                    var xaProduct = BO.Multiply(x, aResidue);
                    x = Calculate(xaProduct, rIndex, modulo);
                }
            }

            return Calculate(x, rIndex, modulo);
        }
    }

    public struct DivisionResult
    {
        public List<bool> Quotient;
        public List<bool> Remainder;
    }
}