using System;
using System.Collections.Concurrent;
using System.Text;
using Hyper.Extensibility.IO;
using Hyper.IO.StringTransforms;

namespace Hyper.IO
{
    /// <summary>
    /// Provides factory methods to create instances of <see cref="IStringTransform"/>.
    /// </summary>
    public static class StringTransform
    {
        private static readonly ConcurrentDictionary<Type, IStringTransform> Transforms = new ConcurrentDictionary<Type, IStringTransform>(); 

        /// <summary>
        /// Returns an instance of <see cref="IStringTransform"/> that transforms strings using the specified <see cref="Encoding"/>.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to use.</param>
        /// <returns></returns>
        public static IStringTransform FromEncoding(Encoding encoding)
        {
            return Transforms.GetOrAdd(encoding.GetType(), new EncodingStringTransform(encoding));
        }

        /// <summary>
        /// Returns an instance of <see cref="IStringTransform"/> that transforms hexadecimal strings.
        /// </summary>
        /// <returns></returns>
        public static IStringTransform GetHexTransform()
        {
            return Transforms.GetOrAdd(typeof(HexStringTransform), new HexStringTransform());
        }

        /// <summary>
        /// Returns an instance of <see cref="IStringTransform"/> that transforms Base64 strings.
        /// </summary>
        /// <returns></returns>
        public static IStringTransform GetBase64Transform()
        {
            return Transforms.GetOrAdd(typeof(Base64StringTransform), new Base64StringTransform());
        }
    }
}
