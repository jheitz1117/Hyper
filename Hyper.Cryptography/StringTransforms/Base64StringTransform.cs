using System;
using Hyper.Extensibility.Cryptography;

namespace Hyper.Cryptography
{
    /// <summary>
    /// Transforms Base64-encoded <see cref="string"/> objects into <see cref="byte"/> arrays and vice versa.
    /// </summary>
    public class Base64StringTransform : IStringTransform
    {
        /// <summary>
        /// Transforms the specified <see cref="byte"/> array into a Base64-encoded <see cref="string"/>.
        /// </summary>
        /// <param name="input">The <see cref="byte"/> array to transform.</param>
        /// <returns></returns>
        public string GetString(byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        /// <summary>
        /// Transforms the specified Base64-encoded <see cref="string"/> into a <see cref="byte"/> array.
        /// </summary>
        /// <param name="input">The <see cref="string"/> to transform.</param>
        /// <returns></returns>
        public byte[] GetBytes(string input)
        {
            return Convert.FromBase64String(input);
        }
    }
}
