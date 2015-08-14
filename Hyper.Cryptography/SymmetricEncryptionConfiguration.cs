using System.Security.Cryptography;
using Hyper.Extensibility.Cryptography;

namespace Hyper.Cryptography
{
    /// <summary>
    /// Contains configuration options for <see cref="SymmetricEncryptionService"/> objects.
    /// </summary>
    public class SymmetricEncryptionConfiguration
    {
        #region Properties

        /// <summary>
        /// Required. Specifies which symmetric algorithm to use for encryption.
        /// </summary>
        public SymmetricAlgorithmType AlgorithmType { get; set; }

        /// <summary>
        /// Required if SymmetricAlgorithmType.Custom is specified. Otherwise, optional.
        /// </summary>
        public SymmetricAlgorithm CustomSymmetricAlgorithm { get; set; }

        /// <summary>
        /// Specifies which CipherMode to use for encryption. Required for all algorithms except SymmetricAlgorithmType.None.
        /// </summary>
        public CipherMode CipherMode { get; set; }

        /// <summary>
        /// Specifies which PaddingMode to use for encryption. Required for all algorithms except SymmetricAlgorithmType.None.
        /// </summary>
        public PaddingMode PaddingMode { get; set; }

        /// <summary>
        /// Specifies an IStringTransform object to use when converting between plaintext strings and bytes. Required for string encryption but not for byte encryption.
        /// </summary>
        public IStringTransform PlainTextTransform { get; set; }

        /// <summary>
        /// Specifies an IStringTransform object to use when converting between ciphertext strings and bytes. Required for string encryption but not for byte encryption.
        /// </summary>
        public IStringTransform CipherTextTransform { get; set; }

        /// <summary>
        /// Specifies an IStringTransform object to use when converting between key strings and bytes. Required unless SymmetricAlgorithmType.None is specified.
        /// </summary>
        public IStringTransform KeyTransform { get; set; }

        /// <summary>
        /// Specifies an IStringTransform object to use when converting between IV strings and bytes. Required unless SymmetricAlgorithmType.None is specified.
        /// </summary>
        public IStringTransform IvTransform { get; set; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of SymmetricEncryptionConfiguration with default settings consisting of:
        /// AES encryption using CBC and PKCS7.
        /// Plaintext strings are encoded using a UTF8 transform.
        /// All other strings (ciphertext, key, and IV strings) are encoded using a hex transform.
        /// </summary>
        public SymmetricEncryptionConfiguration()
        {
            this.AlgorithmType = SymmetricAlgorithmType.Aes;
            this.CipherMode = CipherMode.CBC;
            this.PaddingMode = PaddingMode.PKCS7;
            this.PlainTextTransform = new Utf8StringTransform();
            this.CipherTextTransform = new HexStringTransform();
            this.KeyTransform = new HexStringTransform();
            this.IvTransform = new HexStringTransform();
        }

        #endregion Public Methods
    }
}
