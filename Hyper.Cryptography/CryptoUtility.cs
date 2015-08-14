using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Hyper.Cryptography
{
    internal static class CryptoUtility
    {
        /// <summary>
        /// Converts the specified hex string to the equivalent byte array.
        /// </summary>
        /// <param name="hexString">Hex string to convert.</param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
            { throw new ArgumentNullException("hexString", "Parameter must not be empty or contain only whitespace."); }
            if (!Regex.IsMatch(hexString, "^[a-fA-F0-9]+$") || hexString.Length % 2 == 1)
            { throw new ArgumentException("Parameter must be an even number of characters in the range 0-9 or a-f.", "hexString"); }

            var output = new byte[hexString.Length / 2];
            for (var i = 0; i < hexString.Length; i += 2)
            {
                output[i / 2] = byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber);
            }
            
            return output;
        }

        /// <summary>
        /// Converts the specified byte array to the equivalent hex string.
        /// </summary>
        /// <param name="input">Byte array to convert.</param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] input)
        {
            if (input == null)
            { throw new ArgumentNullException("input"); }

            return string.Join("", input.Select(h => h.ToString("x2")));
        }
    }
}
