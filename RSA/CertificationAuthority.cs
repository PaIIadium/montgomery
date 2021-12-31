namespace RSA
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using ModularExponentiation;

    public static class CertificationAuthority
    {
        public static PublicKey PublicKey { get; private set; }
        private static PrivateKey privateKey;

        public static void Initialize()
        {
            (PublicKey, privateKey) = KeysGenerator.GenerateKeys();
        }

        public static PublicKeyCertificate MakeCertificate(PublicKey publicKey, string userName)
        {
            var certificate = new PublicKeyCertificate {
                PublicKey = publicKey,
                UserName = userName,
            };
            
            var sha256Hash = SHA256.HashData(Encoding.ASCII.GetBytes(certificate.Serialize()));
            var hexHash = Convert.ToHexString(sha256Hash);
            var encryptedHash = Encryptor.EncryptECB(privateKey, hexHash);
            var signature = Converters.BitsToString(encryptedHash);
            certificate.Signature = signature;

            return certificate;
        }
    }

    public class PublicKeyCertificate
    {
        public PublicKey PublicKey;
        public string UserName;
        public string Signature;

        public string Serialize()
        {
            var publicExponentString = Converters.BitsToString(PublicKey.Exponent);
            var moduloString = Converters.BitsToString(PublicKey.Modulo);
            return $"{publicExponentString}/{moduloString}/{UserName}";
        }

        public static PublicKeyCertificate Deserialize(string data)
        {
            var chunks = data.Split('/');
            return new PublicKeyCertificate
            {
                PublicKey = new PublicKey
                {
                    Exponent = Converters.StringToBits(chunks[0]),
                    Modulo = Converters.StringToBits(chunks[1])
                },
                UserName = chunks[2]
            };
        }
    }
}