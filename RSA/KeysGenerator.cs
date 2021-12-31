namespace RSA
{
    using System.Collections.Generic;
    using ModularExponentiation;
    using PrimesGenerator;
    using BO = ModularExponentiation.BinaryOperations;

    public static class KeysGenerator
    {
        private const string PublicExponent = "65537";
        private static readonly List<bool> BinOne = new() {true};
        
        public static (PublicKey publicKey, PrivateKey privateKey) GenerateKeys()
        {
            var randomPrime1 = PrimesGenerator.GeneratePrimeNumber(20);
            var randomPrime2 = PrimesGenerator.GeneratePrimeNumber(20);
            var modulo = BO.Multiply(randomPrime1, randomPrime2);
            var phi = CalculateEulerFn(randomPrime1, randomPrime2);
            var publicExponent = Converters.DecimalToBinary(PublicExponent);
            var secretExponent = MontgomeryCalculator.Inverse(publicExponent, phi);

            var publicKey = new PublicKey
            {
                Exponent = publicExponent,
                Modulo = modulo
            };

            var privateKey = new PrivateKey
            {
                Exponent = secretExponent,
                Modulo = modulo
            };
            return (publicKey, privateKey);
        }
        
        private static List<bool> CalculateEulerFn(List<bool> number1, List<bool> number2)
        {
            var x = BO.Subtract(number1, BinOne);
            var y = BO.Subtract(number2, BinOne);
            return BO.Multiply(x, y);
        }
    }
}