namespace RSA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ModularExponentiation;
    using BO = ModularExponentiation.BinaryOperations;

    public static class Encryptor
    {
        public static List<bool> EncryptECB(IKey key, string message)
        {
            var bits = Converters.StringToBits(message);
            bits = bits.SkipWhile(bit => !bit).ToList();

            var blockSize = key.Modulo.Count - 1;
            var blocksCount = (int)MathF.Ceiling((float)bits.Count / blockSize);
            Tools.PadWithZeros(bits, blocksCount * blockSize);

            var encryptedBits = new List<bool>();

            for (var i = 0; i < bits.Count; i += blockSize)
            {
                var block = bits.GetRange(i, blockSize);
                var number = Tools.SkipFirstZeros(block);
                var encryptedBlock = MontgomeryCalculator.ModularExponentiation(number, key.Exponent, key.Modulo);
                Tools.PadWithZeros(encryptedBlock, blockSize + 1);
                encryptedBits.AddRange(encryptedBlock);
            }

            return encryptedBits;
        }
    }
}