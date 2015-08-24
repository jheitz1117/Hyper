using System.Security.Cryptography;
using System.Text;
using Hyper.Extensibility.IO;
using Hyper.IO;

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
        /// Required if <see cref="SymmetricAlgorithmType"/>.<see cref="SymmetricAlgorithmType.Custom"/> is specified. Otherwise, optional.
        /// </summary>
        public SymmetricAlgorithm CustomSymmetricAlgorithm { get; set; }

        /// <summary>
        /// Specifies which <see cref="CipherMode"/> to use for encryption. Required for all algorithms except <see cref="SymmetricAlgorithmType"/>.<see cref="SymmetricAlgorithmType.None"/>.
        /// </summary>
        public CipherMode CipherMode { get; set; }

        /// <summary>
        /// Specifies which <see cref="PaddingMode"/> to use for encryption. Required for all algorithms except <see cref="SymmetricAlgorithmType"/>.<see cref="SymmetricAlgorithmType.None"/>.
        /// </summary>
        public PaddingMode PaddingMode { get; set; }

        /// <summary>
        /// Specifies an <see cref="IStringTransform"/> instance to use when converting between plaintext strings and bytes. Required for string encryption but not for byte encryption.
        /// </summary>
        public IStringTransform PlainTextTransform { get; set; }

        /// <summary>
        /// Specifies an <see cref="IStringTransform"/> instance to use when converting between ciphertext strings and bytes. Required for string encryption but not for byte encryption.
        /// </summary>
        public IStringTransform CipherTextTransform { get; set; }

        /// <summary>
        /// Specifies an <see cref="IStringTransform"/> instance to use when converting between key strings and bytes. Required unless SymmetricAlgorithmType.None is specified.
        /// </summary>
        public IStringTransform KeyTransform { get; set; }

        /// <summary>
        /// Specifies an <see cref="IStringTransform"/> instance to use when converting between IV strings and bytes. Required unless SymmetricAlgorithmType.None is specified.
        /// </summary>
        public IStringTransform IvTransform { get; set; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of <see cref="SymmetricEncryptionConfiguration"/> with default settings consisting of:
        /// <see cref="SymmetricAlgorithmType.Aes"/> encryption using <see cref="System.Security.Cryptography.CipherMode.CBC"/> and <see cref="System.Security.Cryptography.PaddingMode.PKCS7"/>.
        /// Plaintext strings are encoded using a UTF8 transform.
        /// All other strings (ciphertext, key, and IV strings) are encoded using a hex transform.
        /// </summary>
        public SymmetricEncryptionConfiguration()
        {
            this.AlgorithmType = SymmetricAlgorithmType.Aes;
            this.CipherMode = CipherMode.CBC;
            this.PaddingMode = PaddingMode.PKCS7;
            this.PlainTextTransform = StringTransform.FromEncoding(Encoding.UTF8);
            this.CipherTextTransform = StringTransform.GetHexTransform();
            this.KeyTransform = StringTransform.GetHexTransform();
            this.IvTransform = StringTransform.GetHexTransform();
        }

        #endregion Public Methods
    }
}
