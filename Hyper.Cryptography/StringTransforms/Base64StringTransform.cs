using System;
using Hyper.Extensibility.Cryptography;

namespace Hyper.Cryptography
{
    public class Base64StringTransform : IStringTransform
    {
        public string GetString(byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        public byte[] GetBytes(string input)
        {
            return Convert.FromBase64String(input);
        }
    }
}
