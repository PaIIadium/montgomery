namespace ModularExponentiation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BinaryOperations
    {
        private static readonly List<bool> BinZero = new() {false};
        private static readonly List<bool> BinOne = new() {true};
        
        public static List<bool> Multiply(List<bool> number1, List<bool> number2)
        {
            var result = new bool[number1.Count + number2.Count];
            for (var i = number1.Count - 1; i >= 0; i--)
            {
                var bit1 = number1[i];
                var overflow = false;
                for (var j = number2.Count - 1; j >= 0; j--)
                {
                    var bit2 = number2[j];
                    var bitProduct = bit1 & bit2;
                    var index = i + j + 1;
                    var previousResult = result[index];
                    result[index] ^= bitProduct ^ overflow;
                    overflow = OverflowCalculator.CalculateAdditionOverflow(previousResult, bitProduct, overflow);
                }

                if (overflow) result[i] = true;
            }

            var formattedResult = result.SkipWhile(bit => !bit).ToList();
            if (formattedResult.Count == 0) formattedResult.Add(false);
            return formattedResult;
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
            return new DivisionResult
            {
                Quotient = quotient.SkipWhile(bit => !bit).ToList(),
                Remainder = intermediate
            };
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
            if (number1.Count < number2.Count) (number1, number2) = (number2, number1);
            var resultLength = Math.Max(number1.Count, number2.Count) + 1;
            var result = new bool[resultLength];
            var currentIndex = result.Length - 1;
            var index1 = number1.Count - 1;
            var index2 = number2.Count - 1;
            var lastNonZeroIndex = currentIndex;
            
            var overflow = false;
            
            while (currentIndex > 0)
            {
                var bit1 = number1[index1];
                var bit2 = index2 >= 0 ? number2[index2] : false;
                var sum = bit1 ^ bit2 ^ overflow;
                result[currentIndex] = sum;
                
                if (sum)
                {
                    lastNonZeroIndex = index1 + 1;
                }
                
                overflow = OverflowCalculator.CalculateAdditionOverflow(bit1, bit2, overflow);
                currentIndex--;
                index1--;
                index2--;
            }

            if (overflow)
            {
                result[0] = true;
                lastNonZeroIndex = 0;
            }
            

            return new List<bool>(result[lastNonZeroIndex..]);
        }
        
        public static List<bool> Subtract(List<bool> number1, List<bool> number2, List<bool> modulo = null)
        {
            if (modulo != null && Compare(number1, number2) == -1)
            {
                var diff = Subtract(number2, number1);
                var x = Divide(diff, modulo);
                List<bool> cyclesCount;
                cyclesCount = x.Remainder != BinZero ? Add(x.Quotient, BinOne) : x.Quotient;
                number1 = Add(number1, Multiply(cyclesCount, modulo));
            }
            var result = new bool[number1.Count];
            var index1 = number1.Count - 1;
            var index2 = number2.Count - 1;
            var lastNonZeroIndex = index1;

            var overflow = false;
            while (index1 >= 0)
            {
                var bit1 = number1[index1];
                var bit2 = index2 >= 0 ? number2[index2] : false;
                var delta = bit1 ^ bit2 ^ overflow;
                result[index1] = delta;
                
                if (delta)
                {
                    lastNonZeroIndex = index1;
                }
                
                overflow = OverflowCalculator.CalculateSubtractionOverflow(bit1, bit2, overflow);
                
                index1--;
                index2--;
            }

            return new List<bool>(result[lastNonZeroIndex..]);
        }

        public static List<bool> DivideByBinaryPower(List<bool> number, int binaryPowerIndex)
        {
            if (number.Count < binaryPowerIndex) return BinZero;
            return number.GetRange(0, number.Count - binaryPowerIndex);
        }
    }
}