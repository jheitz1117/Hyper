using System.IO;
using System.Security.Cryptography;
using System.Text;
using Hyper.Extensibility.IO;
using Hyper.IO;

namespace Hyper.Cryptography
{
    /// <summary>
    /// MD5 hash wrapper for strings, files, and byte arrays.
    /// </summary>
    public class Md5Generator
    {
        // TODO: By default, use Utf8 encoding for plaintext and Hex for ciphertext like we were doing before. This should be cleaned up later.
        
        /// <summary>
        /// Specifies an <see cref="IStringTransform"/> instance to use when converting between plaintext strings and bytes. Required for string hashing but not for byte hashing.
        /// </summary>
        private static readonly IStringTransform PlainTextTransform = StringTransform.FromEncoding(Encoding.UTF8);

        /// <summary>
        /// Specifies an <see cref="IStringTransform"/> instance to use when converting between ciphertext strings and bytes. Required for string hashing but not for byte hashing.
        /// </summary>
        private static readonly IStringTransform CipherTextTransform = StringTransform.GetHexTransform();

        /// <summary>
        /// Returns the uppercase MD5 hash string for the specified string value.
        /// </summary>
        /// <param name="plainValue">String value to hash.</param>
        /// <returns></returns>
        public static string GetMd5(string plainValue)
        {
            return GetMd5(PlainTextTransform.GetBytes(plainValue));
        }

        /// <summary>
        /// Returns the uppercase MD5 hash string for the specified file.
        /// </summary>
        /// <param name="filepath">Full path to the file to hash.</param>
        /// <returns></returns>
        public static string GetFileMd5(string filepath)
        {
            return GetMd5(File.ReadAllBytes(filepath));
        }

        /// <summary>
        /// Returns the uppercase MD5 hash string for the specified byte array.
        /// </summary>
        /// <param name="inputBytes">Byte array to hash.</param>
        /// <returns></returns>
        private static string GetMd5(byte[] inputBytes)
        {
            return CipherTextTransform.GetString(MD5.Create().ComputeHash(inputBytes)).ToUpper();
        }
    }
}
