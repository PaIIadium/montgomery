namespace ModularExponentiation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BinaryOperations
    {
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
                    overflow = Dictionaries.AdderTable[previousResult][bitProduct][overflow];
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
            var number2List = new List<bool>(number2);
            var isNeededZeroAtEnd = false;
            while (currentBitIndex < number1.Count)
            {
                var nextBit = number1[currentBitIndex];
                if (intermediate.Count == 0 && !nextBit)
                {
                    quotient.Add(false);
                    currentBitIndex++;
                    continue;
                }
                
                intermediate.Add(nextBit);
                var comparingResult = Compare(intermediate, number2List);
                if (comparingResult is 1 or 0)
                {
                    quotient.Add(true);
                    intermediate = Subtract(intermediate, number2);
                    if (intermediate.Count == 1 && !intermediate[0])
                    {
                        intermediate.Clear();
                    }

                    isNeededZeroAtEnd = false;
                }
                else
                {
                    isNeededZeroAtEnd = true;
                }

                currentBitIndex++;
            }

            if (isNeededZeroAtEnd) quotient.Add(false);
            if (intermediate.Count == 0) intermediate.Add(false);
            return new DivisionResult
            {
                Quotient = quotient,
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
                
                overflow = Dictionaries.AdderTable[bit1][bit2][overflow];
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
            if (modulo != null && Compare(number1, number2) == -1) number1 = Add(number1, modulo); 
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
                
                overflow = Dictionaries.SubtractorTable[bit1][bit2][overflow];
                
                index1--;
                index2--;
            }

            return new List<bool>(result[lastNonZeroIndex..]);
        }

        public static List<bool> DivideByBinaryPower(List<bool> number, int binaryPowerIndex)
        {
            return number.GetRange(0, number.Count - binaryPowerIndex);
        }
    }
}