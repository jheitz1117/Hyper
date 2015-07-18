using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hyper.Cryptography
{
    public class MD5Generator
    {
        /// <summary>
        /// Returns the uppercase MD5 hash string for the specified string value.
        /// </summary>
        /// <param name="plainValue">String value to hash.</param>
        /// <returns></returns>
        public static string GetMD5(string plainValue)
        {
            return GetMD5(Encoding.UTF8.GetBytes(plainValue));
        }

        /// <summary>
        /// Returns the uppercase MD5 hash string for the specified file.
        /// </summary>
        /// <param name="filepath">Full path to the file to hash.</param>
        /// <returns></returns>
        public static string GetFileMD5(string filepath)
        {
            return GetMD5(File.ReadAllBytes(filepath));
        }

        /// <summary>
        /// Returns the uppercase MD5 hash string for the specified byte array.
        /// </summary>
        /// <param name="inputBytes">Byte array to hash.</param>
        /// <returns></returns>
        private static string GetMD5(byte[] inputBytes)
        {
            return CryptoUtility.BytesToHexString(MD5.Create().ComputeHash(inputBytes));
        }
    }
}
