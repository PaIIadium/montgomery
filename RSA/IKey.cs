namespace RSA
{
    using System.Collections.Generic;

    public interface IKey
    {
        public List<bool> Exponent { get; set; }
        public List<bool> Modulo { get; set; }
    }
    
    public class PublicKey : IKey
    {
        public List<bool> Exponent { get; set; }
        public List<bool> Modulo { get; set; }
    }

    public class PrivateKey : IKey
    {
        public List<bool> Exponent { get; set; }
        public List<bool> Modulo { get; set; }
    }
}