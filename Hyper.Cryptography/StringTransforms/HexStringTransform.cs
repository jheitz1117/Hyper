using Hyper.Extensibility.Cryptography;

namespace Hyper.Cryptography
{
    public class HexStringTransform : IStringTransform
    {
        public string GetString(byte[] input)
        {
            return CryptoUtility.BytesToHexString(input);
        }

        public byte[] GetBytes(string input)
        {
            return CryptoUtility.HexStringToBytes(input);
        }
    }
}
