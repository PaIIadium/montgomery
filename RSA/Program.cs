namespace RSA
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using ModularExponentiation;

    class Program
    {
        private static void Main(string[] args)
        {
            Converters.Initialize();
            CertificationAuthority.Initialize();
            var (publicKey, privateKey) = KeysGenerator.GenerateKeys();
            var certificate = CertificationAuthority.MakeCertificate(publicKey, "Kuchin");

            if (CheckCertificate(certificate))
            {
                var encryptedMsg = Encryptor.EncryptECB(certificate.PublicKey, "Hello world");
                var decryptedMsg = Decryptor.Decrypt(privateKey, encryptedMsg);
                Console.WriteLine(decryptedMsg);
            }
            else
            {
                Console.WriteLine("Certificate is invalid");
            }
        }

        private static bool CheckCertificate(PublicKeyCertificate certificate)
        {
            var CAPublicKey = CertificationAuthority.PublicKey;
            var signature = certificate.Signature;
            var encryptedHash = Converters.StringToBits(signature);
            var decryptedHash = Decryptor.Decrypt(CAPublicKey, encryptedHash);

            var certificateHash = SHA256.HashData(Encoding.ASCII.GetBytes(certificate.Serialize()));
            var hash = Convert.ToHexString(certificateHash);
            return decryptedHash == hash;
        }
    }
}