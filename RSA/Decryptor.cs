namespace RSA
{
    using System.Collections.Generic;
    using ModularExponentiation;

    public static class Decryptor
    {
        public static string Decrypt(IKey key, List<bool> encryptedMessage)
        {
            var blockSize = key.Modulo.Count;

            var decryptedBits = new List<bool>();
            for (var i = 0; i < encryptedMessage.Count; i += blockSize)
            {
                var block = encryptedMessage.GetRange(i, blockSize);
                var number = Tools.SkipFirstZeros(block);
                var decryptedBlock = MontgomeryCalculator.ModularExponentiation(number, key.Exponent, key.Modulo);
                Tools.PadWithZeros(decryptedBlock, blockSize - 1);
                decryptedBits.AddRange(decryptedBlock);
            }
            
            var alignedResult = Align(decryptedBits);
            return Converters.BitsToString(alignedResult);
        }

        private static List<bool> Align(List<bool> bits)
        {
            const int byteLength = 8;
            var withoutFirstZeros = Tools.SkipFirstZeros(bits);
            var residue = withoutFirstZeros.Count % byteLength;
            var requiredSize = withoutFirstZeros.Count + (byteLength - residue);
            Tools.PadWithZeros(withoutFirstZeros, requiredSize);
            return withoutFirstZeros;
        }
    }
}