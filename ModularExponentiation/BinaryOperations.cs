namespace ModularExponentiation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BinaryOperations
    {
        private static readonly List<bool> BinZero = new() {false};
        private static readonly List<bool> BinOne = new() {true};
        
        public static List<bool> Multiply(List<bool> firstBin, List<bool> secondBin)
        {
            var first = Converters.BinaryToUintArr(firstBin);
            var second = Converters.BinaryToUintArr(secondBin);

            var result = new uint[first.Length + second.Length];
            var firstCopy = new uint[result.Length];
            first.CopyTo(firstCopy, result.Length - first.Length);
            
            for (var i = second.Length - 1; i >= 0; i--)
            {
                var secondInt = second[i];
                for (var k = 31; k >= 0; k--)
                {
                    var bit = Tools.GetBit(secondInt, k);
                    if (bit)
                    {
                        AddTo(result, firstCopy);
                    }
                    ShiftByOne(firstCopy);
                }
            }
            return Converters.UintArrToBinary(result);
        }

        private static void ShiftByOne(uint[] num)
        {
            var previousShiftedBit = false;
            for (var i = num.Length - 1; i >= 0; i--)
            {
                var chunk = num[i];
                var currentShiftedBit = Tools.GetBit(chunk, 0);
                chunk <<= 1;
                if (previousShiftedBit) chunk += 1;
                previousShiftedBit = currentShiftedBit;
                num[i] = chunk;
            }
        }
        
        private static void AddTo(uint[] result, uint[] term)
        {
            var overflowBit = false;
            for (var i = result.Length - 1; i >= 0; i--)
            {
                var termInt = term[i];
                unchecked
                {
                    if (overflowBit)
                    {
                        result[i] += 1;
                        overflowBit = result[i] == 0;
                    }

                    result[i] += termInt;
                    overflowBit = overflowBit || result[i] < termInt;
                }
            }
        }
        
        public static DivisionResult Divide(List<bool> number1, List<bool> number2)
        {
            var quotient = new List<bool>();
            var currentBitIndex = 0;
            var intermediate = new List<bool>();
            while (currentBitIndex < number1.Count)
            {
                var nextBit = number1[currentBitIndex];
                if (Compare(intermediate, BinZero) == 0)
                {
                    if (nextBit) intermediate[0] = true;
                }
                else
                {
                    intermediate.Add(nextBit);
                }
                
                var comparingResult = Compare(intermediate, number2);
                if (comparingResult is 1 or 0)
                {
                    quotient.Add(true);
                    intermediate = Subtract(intermediate, number2);
                }
                else
                {
                    quotient.Add(false);
                }

                currentBitIndex++;
            }

            if (intermediate.Count == 0) intermediate.Add(false);
            var res = new DivisionResult
            {
                Quotient = quotient.SkipWhile(bit => !bit).ToList(),
                Remainder = intermediate
            };
            if (res.Quotient.Count == 0) res.Quotient.Add(false);
            return res;
        }

        public static int Compare(List<bool> number1, List<bool> number2)
        {
            if (number1.Count > number2.Count) return 1;
            if (number1.Count < number2.Count) return -1;
            for (var i = 0; i < number1.Count; i++)
            {
                if (number1[i] && !number2[i]) return 1;
                if (!number1[i] && number2[i]) return -1;
            }

            return 0;
        }

        public static List<bool> Add(List<bool> number1, List<bool> number2)
        {
            var first = Converters.BinaryToUintArr(number1);
            var second = Converters.BinaryToUintArr(number2);
        
            var result = new uint[Math.Max(first.Length, second.Length) + 1];
            var secondCopy = new uint[result.Length];
            second.CopyTo(secondCopy, result.Length - second.Length);
            
            first.CopyTo(result, result.Length - first.Length);
            AddTo(result, secondCopy);
            return Converters.UintArrToBinary(result);
        }
        
        public static List<bool> Subtract(List<bool> number1, List<bool> number2, List<bool> modulo = null)
        {
            if (modulo != null && Compare(number1, number2) == -1)
            {
                var diff = Subtract(number2, number1);
                var x = Divide(diff, modulo);
                var cyclesCount = x.Remainder != BinZero ? Add(x.Quotient, BinOne) : x.Quotient;
                number1 = Add(number1, Multiply(cyclesCount, modulo));
            }
            
            var first = Converters.BinaryToUintArr(number1);
            var second = Converters.BinaryToUintArr(number2);
            
            var secondCopy = new uint[first.Length];
            second.CopyTo(secondCopy, first.Length - second.Length);
        
            SubtractFrom(first, secondCopy);
            return Converters.UintArrToBinary(first);
        }
        
        private static void SubtractFrom(uint[] result, uint[] subtractor)
        {
            var overflowBit = false;
            for (var i = result.Length - 1; i >= 0; i--)
            {
                var subtractorInt = subtractor[i];
                unchecked
                {
                    if (overflowBit)
                    {
                        result[i] -= 1;
                        overflowBit = result[i] == 0xFFFFFFFFu;
                    }

                    var oldResult = result[i];
                    result[i] -= subtractorInt;
                    overflowBit = overflowBit || result[i] > oldResult;
                }
            }
        }

        public static List<bool> DivideByBinaryPower(List<bool> number, int binaryPowerIndex)
        {
            if (number.Count < binaryPowerIndex) return BinZero;
            return number.GetRange(0, number.Count - binaryPowerIndex);
        }
    }
}