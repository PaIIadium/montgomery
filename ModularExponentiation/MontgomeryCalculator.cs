﻿namespace ModularExponentiation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BO = BinaryOperations;
    using DO = DecimalOperations;
    
    public class MontgomeryCalculator
    {
        private static void Entry(string[] args)
        {
            var a = args[0];
            var b = args[1];
            var n = args[2];
            Converters.Initialize();
            var aBin = Converters.DecimalToBinary(a);
            var bBin = Converters.DecimalToBinary(b);
            var nBin = Converters.DecimalToBinary(n);

            var product = Multiply(aBin, bBin, nBin);
            var decimalProduct = Converters.BinaryToDecimal(product);
            Console.WriteLine($"{a} * {b} (mod {n}) = {decimalProduct}");
            var modExp = ModularExponentiation(aBin, bBin, nBin);
            var decimalModExp = Converters.BinaryToDecimal(modExp);
            Console.WriteLine($"{a} ^ {b} (mod {n}) = {decimalModExp}");
        }

        public static List<bool> Multiply(List<bool> a, List<bool> b, List<bool> n)
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
            if (binaryNumber.Count < binaryPowerIndex) return binaryNumber;
            var result = binaryNumber
                .GetRange(binaryNumber.Count - binaryPowerIndex, binaryPowerIndex)
                .SkipWhile(bit => !bit).ToList();
            if (result.Count == 0) result.Add(false);
            return result;
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
        
        public static List<bool> ModularExponentiation(List<bool> number, List<bool> exponent, List<bool> modulo)
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