using System.Text;
using Hyper.Extensibility.IO;

namespace Hyper.IO.StringTransforms
{
    /// <summary>
    /// Transforms <see cref="string"/> objects into <see cref="byte"/> arrays and vice versa using a specific <see cref="_encoding"/>.
    /// </summary>
    internal class EncodingStringTransform : IStringTransform
    {
        private readonly Encoding _encoding;

        /// <summary>
        /// Initializes a new instance of <see cref="EncodingStringTransform"/> using the specified <see cref="Encoding"/>.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to use.</param>
        public EncodingStringTransform(Encoding encoding)
        {
            _encoding = encoding;
        }

        /// <summary>
        /// Transforms the specified <see cref="byte"/> array into a <see cref="string"/> using the <see cref="Encoding"/> provided in the constructor.
        /// </summary>
        /// <param name="input">The <see cref="byte"/> array to transform.</param>
        /// <returns></returns>
        public string GetString(byte[] input)
        {
            return _encoding.GetString(input);
        }

        /// <summary>
        /// Transforms the specified <see cref="string"/> into a <see cref="byte"/> array using the <see cref="Encoding"/> provided in the constructor.
        /// </summary>
        /// <param name="input">The <see cref="string"/> to transform.</param>
        /// <returns></returns>
        public byte[] GetBytes(string input)
        {
            return _encoding.GetBytes(input);
        }
    }
}
