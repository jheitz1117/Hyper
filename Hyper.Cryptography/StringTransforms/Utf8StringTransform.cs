using System.Text;
using Hyper.Extensibility.Cryptography;

namespace Hyper.Cryptography
{
    /// <summary>
    /// Transforms <see cref="string"/> objects into <see cref="byte"/> arrays and vice versa using the UTF-8 encoding.
    /// </summary>
    public class Utf8StringTransform : IStringTransform
    {
        /// <summary>
        /// Transforms the specified <see cref="byte"/> array into a <see cref="string"/> using the UTF-8 encoding.
        /// </summary>
        /// <param name="input">The <see cref="byte"/> array to transform.</param>
        /// <returns></returns>
        public string GetString(byte[] input)
        {
            return Encoding.UTF8.GetString(input);
        }

        /// <summary>
        /// Transforms the specified <see cref="string"/> into a <see cref="byte"/> array using the UTF-8 encoding.
        /// </summary>
        /// <param name="input">The <see cref="string"/> to transform.</param>
        /// <returns></returns>
        public byte[] GetBytes(string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }
    }
}
