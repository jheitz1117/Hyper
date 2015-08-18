using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Hyper.Extensibility.IO;

namespace Hyper.IO.StringTransforms
{
    /// <summary>
    /// Transforms <see cref="string"/> objects containing only hexidecimal digits into <see cref="byte"/> arrays and vice versa.
    /// </summary>
    internal class HexStringTransform : IStringTransform
    {
        /// <summary>
        /// Transforms the specified <see cref="byte"/> array into a hexidecimal <see cref="string"/>.
        /// </summary>
        /// <param name="input">The <see cref="byte"/> array to transform.</param>
        /// <returns></returns>
        public string GetString(byte[] input)
        {
            if (input == null)
            { throw new ArgumentNullException("input"); }

            return string.Join("", input.Select(h => h.ToString("x2")));
        }

        /// <summary>
        /// Transforms the specified hexidecimal <see cref="string"/> into a <see cref="byte"/> array.
        /// </summary>
        /// <param name="input">The hexidecimal <see cref="string"/> to transform.</param>
        /// <returns></returns>
        public byte[] GetBytes(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            { throw new ArgumentNullException("input", "Parameter must not be empty or contain only whitespace."); }
            if (!Regex.IsMatch(input, "^[a-fA-F0-9]+$") || input.Length % 2 == 1)
            { throw new ArgumentException("Parameter must be an even number of characters in the range 0-9 or a-f.", "input"); }

            var output = new byte[input.Length / 2];
            for (var i = 0; i < input.Length; i += 2)
            {
                output[i / 2] = byte.Parse(input.Substring(i, 2), NumberStyles.HexNumber);
            }

            return output;
        }
    }
}
