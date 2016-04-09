using Hyper.Cryptography;
using Hyper.Extensibility.IO;

namespace Hyper.Core.Cryptography
{
    /// <summary>
    /// Symmetric algorithms supported by the <see cref="SymmetricEncryptionService"/>.
    /// </summary>
    public enum SymmetricAlgorithmType
    {
        /// <summary>
        /// Indicates that no encryption should be used and the bytes/strings should simply be transformed using the specified <see cref="IStringTransform"/>.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that a user-defined algorithm should be used.
        /// </summary>
        Custom = 1,

        /// <summary>
        /// Indicates that the Triple DES algorithm should be used.
        /// </summary>
        TripleDes = 2,

        /// <summary>
        /// Indicates that the DES algorithm should be used.
        /// </summary>
        Des = 3,

        /// <summary>
        /// Indicates that the RC2 algorithm should be used.
        /// </summary>
        Rc2 = 4,

        /// <summary>
        /// Indicates that the Rijndael algorithm should be used.
        /// </summary>
        Rijndael = 5,

        /// <summary>
        /// Indicates that the AES algorithm should be used.
        /// </summary>
        Aes = 6
    }
}
