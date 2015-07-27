using System.Text;
using Hyper.Extensibility.Cryptography;

namespace Hyper.Cryptography
{
    public class Utf8StringTransform : IStringTransform
    {
        public string GetString(byte[] input)
        {
            return Encoding.UTF8.GetString(input);
        }

        public byte[] GetBytes(string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }
    }
}
