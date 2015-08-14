using Hyper.Extensibility.Cryptography;

namespace Hyper.Cryptography
{
    /// <summary>
    /// Transforms <see cref="string"/> objects containing only hexidecimal digits into <see cref="byte"/> arrays and vice versa.
    /// </summary>
    public class HexStringTransform : IStringTransform
    {
        /// <summary>
        /// Transforms the specified <see cref="byte"/> array into a hexidecimal <see cref="string"/>.
        /// </summary>
        /// <param name="input">The <see cref="byte"/> array to transform.</param>
        /// <returns></returns>
        public string GetString(byte[] input)
        {
            return CryptoUtility.BytesToHexString(input);
        }

        /// <summary>
        /// Transforms the specified hexidecimal <see cref="string"/> into a <see cref="byte"/> array.
        /// </summary>
        /// <param name="input">The hexidecimal <see cref="string"/> to transform.</param>
        /// <returns></returns>
        public byte[] GetBytes(string input)
        {
            return CryptoUtility.HexStringToBytes(input);
        }
    }
}
