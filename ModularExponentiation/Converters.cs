﻿namespace ModularExponentiation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DO = DecimalOperations;

    public class Converters
    {
        private static readonly Dictionary<int, string> BinaryPowers = new();
        private static readonly Dictionary<int, int> BinaryPowersLengths = new();
        private static readonly double Log2_10 = Math.Log2(10);
        private const int MaxBinaryPower = 2048;
        private const int ByteLength = 8;

        public static void Initialize()
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
        
        public static string BinaryToDecimal(List<bool> binary)
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
        
        public static List<bool> DecimalToBinary(string number)
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

        public static List<bool> StringToBits(string str)
        {
            var result = new List<bool>(str.Length * ByteLength);
            foreach (var character in str)
            {
                var decimalRepresentation = (byte) character;
                var binaryRepresentation = DecimalToBinary(decimalRepresentation.ToString());
                var binaryArray = new bool[ByteLength];
                binaryRepresentation.CopyTo(binaryArray, ByteLength - binaryRepresentation.Count);
                result.AddRange(binaryArray);
            }

            return result;
        }

        public static string BitsToString(List<bool> bits)
        {
            bits = bits.ToList();
            var result = "";
            var residue = bits.Count % ByteLength;
            if (residue != 0)
            {
                var additionalBits = new bool[ByteLength - residue];
                bits.InsertRange(0, additionalBits);   
            }

            for (var i = 0; i < bits.Count; i += ByteLength)
            {
                var binaryCharacter = bits.GetRange(i, ByteLength);
                var character = (char)byte.Parse(BinaryToDecimal(binaryCharacter));
                result += character;
            }

            return result;
        }
    }
}