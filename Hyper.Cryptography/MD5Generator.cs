﻿using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hyper.Cryptography
{
    /// <summary>
    /// MD5 hash wrapper for strings, files, and byte arrays.
    /// </summary>
    public class Md5Generator
    {
        /// <summary>
        /// Returns the uppercase MD5 hash string for the specified string value.
        /// </summary>
        /// <param name="plainValue">String value to hash.</param>
        /// <returns></returns>
        public static string GetMd5(string plainValue)
        {
            return GetMd5(Encoding.UTF8.GetBytes(plainValue));
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
            return CryptoUtility.BytesToHexString(MD5.Create().ComputeHash(inputBytes)).ToUpper();
        }
    }
}
