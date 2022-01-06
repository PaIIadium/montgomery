namespace AES
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;

    class Program
    {
        private const int KeySize = 16;
        
        private static readonly List<byte> Iv = new()
            {0x13, 0x76, 0xFD, 0x61, 0x10, 0x99, 0x61, 0xAE, 0x13, 0x76, 0xFD, 0x61, 0x10, 0x99, 0x61, 0xAE};

        private static readonly RandomNumberGenerator RandomBytesGenerator = RandomNumberGenerator.Create();

        private static void Main(string[] args)
        {
            GaloisMultiplication.Initialize();
            
            CBCEncryptionExample();
            ECBEncryptionExample();
            SignMessageExample();
        }

        private static void CBCEncryptionExample()
        {
            var msg = StringToBytes("Example 1. This message is encrypted in CBC mode");
            var randomBytesArray = new byte[KeySize];
            RandomBytesGenerator.GetBytes(randomBytesArray);
            var key = randomBytesArray.ToList();
            
            var encryptedMsg = Encryptor.Encrypt(key, msg, EncryptionMode.CBC, Iv);
            var decryptedMsg = BytesToString(Decryptor.Decrypt(key, encryptedMsg, EncryptionMode.CBC, Iv));
            
            Console.WriteLine(decryptedMsg);
        }
        
        private static List<byte> StringToBytes(string str)
        {
            return str.ToCharArray().Select(ch => (byte) ch).ToList();
        }

        private static string BytesToString(List<byte> bytes)
        {
            return new string(bytes.Select(byt => (char) byt).ToArray());
        }
        
        private static void ECBEncryptionExample()
        {
            var msg = StringToBytes("Example 2. This message is encrypted in ECB mode");
            var randomBytesArray = new byte[KeySize];
            RandomBytesGenerator.GetBytes(randomBytesArray);
            var key = randomBytesArray.ToList();
            
            var encryptedMsg = Encryptor.Encrypt(key, msg, EncryptionMode.ECB);
            var decryptedMsg = BytesToString(Decryptor.Decrypt(key, encryptedMsg, EncryptionMode.ECB));
            
            Console.WriteLine(decryptedMsg);
        }

        private static void SignMessageExample()
        {
            var randomBytesArray = new byte[KeySize];
            RandomBytesGenerator.GetBytes(randomBytesArray);
            var aKey = randomBytesArray.ToList();
            RandomBytesGenerator.GetBytes(randomBytesArray);
            var bKey = randomBytesArray.ToList();
            SignatureMediator.RegisterUser("Alice", aKey);
            SignatureMediator.RegisterUser("Bob", bKey);

            var aMessage = StringToBytes("Message for Bob from Alice");
            var encryptedAMessage = Encryptor.Encrypt(aKey, aMessage, EncryptionMode.ECB);
            var signedMessage =
                SignatureMediator.SignMessage("Alice", "Bob", encryptedAMessage, EncryptionMode.ECB);

            var decryptedMessage = BytesToString(Decryptor.Decrypt(bKey, signedMessage, EncryptionMode.ECB));
            Console.WriteLine(decryptedMessage);
        }
    }
}