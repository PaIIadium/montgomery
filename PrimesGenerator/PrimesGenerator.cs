namespace PrimesGenerator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ModularExponentiation;
    using BO = ModularExponentiation.BinaryOperations;

    public class PrimesGenerator
    {
        private static readonly List<bool> BinOne = new() {true};
        private static readonly List<bool> BinTwo = new() {true, false};
        
        private static void Main(string[] args)
        {
            var bitsCount = int.Parse(args[0]);
            Converters.Initialize();
            var primeNumber = GeneratePrimeNumber(bitsCount);
            Console.WriteLine(Converters.BinaryToDecimal(primeNumber));
        }

        public static List<bool> GeneratePrimeNumber(int bitsCount)
        {
            var rangeStart = new List<bool>(new bool[bitsCount]) {[0] = true};
            var rangeEnd = new List<bool>(new bool[bitsCount]);
            for (var i = 0; i < bitsCount; i++) rangeEnd[i] = true;
            var randomNumber = GetRandomBinaryInRange(rangeStart, rangeEnd);
            randomNumber[^1] = true;
            while (true)
            {
                var probability = MillerTest(randomNumber, bitsCount);
                if (probability != 0) return randomNumber;
                randomNumber = BO.Add(randomNumber, BinTwo);
            }
        }

        private static float MillerTest(List<bool> number, int roundsCount)
        {
            var (s, t) = GetSAndT(number);
            var rangeStart = BinTwo;
            var rangeEnd = BO.Subtract(number, BinTwo);
            var nMinusOne = number.ToList();
            nMinusOne[^1] = false;
            
            for (var i = 0; i < roundsCount; i++)
            {
                var randomNumber = GetRandomBinaryInRange(rangeStart, rangeEnd);
                var x = MontgomeryCalculator.ModularExponentiation(randomNumber, t, number);
                
                if (BO.Compare(x, BinOne) == 0 || BO.Compare(x, nMinusOne) == 0) continue;
                var isComposite = true;
                for (var j = 0; j < s; j++)
                {
                    x = MontgomeryCalculator.ModularExponentiation(x, BinTwo, number);
                    if (BO.Compare(x, BinOne) == 0)
                    {
                        return 0;
                    }

                    if (BO.Compare(x, nMinusOne) == 0)
                    {
                        isComposite = false;
                        break;
                    }
                }
                if (!isComposite) continue;
                
                return 0;
            }
            return 100 - 1f / MathF.Pow(4, roundsCount) * 100;
        }

        private static bool IsEven(List<bool> number)
        {
            return !number.Last();
        }

        private static (int, List<bool>) GetSAndT(List<bool> number)
        {
            number[^1] = false;
            number.Reverse();
            var zeros = number.TakeWhile(bit => !bit);
            var s = zeros.Count();
            number.Reverse();
            var t = number.GetRange(0, number.Count - s);
            number[^1] = true;
            return (s, t);
        }

        private static List<bool> GetRandomBinaryInRange(List<bool> start, List<bool> end)
        {
            var delta = BO.Subtract(end, start);
            var random = new Random();
            var randomBytesCount = (int)MathF.Ceiling(delta.Count / 8f);

            var randomBytes = new byte[randomBytesCount];
            random.NextBytes(randomBytes);

            var randomBits = new List<bool>();
            foreach (var randomByte in randomBytes[..^1])
            {
                var bits = new bool[8];
                var bitArray = new BitArray(new [] {randomByte});
                bitArray.CopyTo(bits, 0);
                randomBits.AddRange(bits);
            }

            var lastRandomByte = randomBytes[^1];
            var lastRandomBitArray = new BitArray(new [] {lastRandomByte});
            
            var bitsRemaining = delta.Count - randomBits.Count;
            var lastRandomBits = new bool[8];
            lastRandomBitArray.CopyTo(lastRandomBits, 0);
            randomBits.AddRange(lastRandomBits[..bitsRemaining]);
            randomBits[0] = true;
            if (BO.Compare(randomBits, delta) == 1) randomBits.RemoveAt(randomBits.Count - 1);
            return BO.Add(start, randomBits);
        }
    }
}