namespace AES
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class SignatureMediator
    {
        private static Dictionary<string,  List<byte>> usersKeys = new();

        private static readonly List<byte> signatureString =
            "\r\nThis message was signed by ".ToCharArray().Select(ch => (byte) ch).ToList();

        public static void RegisterUser(string username, List<byte> secretKey)
        {
            usersKeys[username] = secretKey;
        }

        public static List<byte> SignMessage(string from, string to, List<byte> encryptedMessage, EncryptionMode mode, List<byte> iv = null)
        {
            if (usersKeys.TryGetValue(from, out var senderKey))
            {
                if (usersKeys.TryGetValue(to, out var receiverKey))
                {
                    var decryptedMessage = Decryptor.Decrypt(senderKey, encryptedMessage, mode, iv);
                    var signature = signatureString.ToList();
                    signature.AddRange(from.ToCharArray().Select(ch => (byte) ch));
                    decryptedMessage.AddRange(signature);
                    return Encryptor.Encrypt(receiverKey, decryptedMessage, mode, iv);
                }

                Console.WriteLine("Receiver with this name was not found");
            }
            else
            {
                Console.WriteLine("Sender with this name was not found");
            }

            return null;
        }
    }
}