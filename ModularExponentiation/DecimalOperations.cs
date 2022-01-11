namespace ModularExponentiation
{
    using System;

    public class DecimalOperations
    {
        public static int Compare(string number1, string number2)
        {
            if (number1.Length > number2.Length) return 1;
            if (number1.Length < number2.Length) return -1;
            
            for (var i = 0; i < number1.Length; i++)
            {
                var digit1 = number1[i];
                var digit2 = number2[i];
                if (digit1 > digit2) return 1;
                if (digit1 < digit2) return -1;
            }

            return 0;
        }

        public static string Add(string number1, string number2)
        {
            if (number1.Length < number2.Length) (number1, number2) = (number2, number1);
            var resultLength = Math.Max(number1.Length, number2.Length) + 1;

            var result = new char[resultLength];
            var currentIndex = result.Length - 1;
            var index1 = number1.Length - 1;
            var index2 = number2.Length - 1;
            var lastNonZeroIndex = currentIndex;

            var overflow = false;
            while (currentIndex > 0)
            {
                var char1 = number1[index1];
                var char2 = index2 >= 0 ? number2[index2] : '0';
                var sum = Converters.CharToByte[char1] + Converters.CharToByte[char2];
                if (overflow) sum += 1;
                if (sum >= 10)
                {
                    overflow = true;
                    sum -= 10;
                    lastNonZeroIndex = currentIndex;
                }
                else
                {
                    overflow = false;
                    if (sum != 0) lastNonZeroIndex = currentIndex;
                }
                
                result[currentIndex] = Converters.ByteToChar[(byte)sum];
                currentIndex--;
                index1--;
                index2--;
            }

            if (overflow)
            {
                result[0] = '1';
                lastNonZeroIndex = 0;
            }
            
            return new string(result[lastNonZeroIndex..]);
        }
        
        public static string Subtract(string number1, string number2)
        {
            var result = new char[number1.Length];
            var index1 = number1.Length - 1;
            var index2 = number2.Length - 1;
            var lastNonZeroIndex = index1;

            var overflow = false;
            while (index1 >= 0)
            {
                var char1 = number1[index1];
                var char2 = index2 >= 0 ? number2[index2] : '0';
                var delta = char1 - char2;
                if (overflow) delta -= 1;
                if (delta < 0)
                {
                    overflow = true;
                    delta += 10;
                    lastNonZeroIndex = index1;
                }
                else
                {
                    overflow = false;
                    if (delta != 0) lastNonZeroIndex = index1;
                }
                
                result[index1] = Converters.ByteToChar[(byte)delta];
                index1--;
                index2--;
            }

            return new string(result[lastNonZeroIndex..]);
        }
    }
}