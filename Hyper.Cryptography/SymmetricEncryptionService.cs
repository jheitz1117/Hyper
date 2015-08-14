using System;
using System.IO;
using System.Security.Cryptography;
using Hyper.Extensibility.Cryptography;

namespace Hyper.Cryptography
{
    /// <summary>
    /// Provides encryption services using the options in the specified <see cref="SymmetricEncryptionConfiguration"/> instance.
    /// </summary>
    public class SymmetricEncryptionService
    {
        private readonly SymmetricAlgorithm _algorithm;
        private readonly SymmetricEncryptionConfiguration _config;

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of <see cref="SymmetricEncryptionService"/> with the specified configuration.
        /// </summary>
        /// <param name="config">The <see cref="SymmetricEncryptionConfiguration"/> to use for encryption.</param>
        public SymmetricEncryptionService(SymmetricEncryptionConfiguration config)
        {
            if (config == null)
            { throw new ArgumentNullException("config"); }
            
            _config = config;

            switch (config.AlgorithmType)
            {
                case SymmetricAlgorithmType.None: // effectively turns off encryption, so output is equal to input, except for any transformations done by the IStringTransform objects
                    _algorithm = null; 
                    break;
                case SymmetricAlgorithmType.TripleDes:
                    _algorithm = new TripleDESCryptoServiceProvider();
                    break;
                case SymmetricAlgorithmType.Des:
                    _algorithm = new DESCryptoServiceProvider();
                    break;
                case SymmetricAlgorithmType.Rc2:
                    _algorithm = new RC2CryptoServiceProvider();
                    break;
                case SymmetricAlgorithmType.Rijndael:
                    _algorithm = new RijndaelManaged();
                    break;
                case SymmetricAlgorithmType.Aes:
                    _algorithm = new AesCryptoServiceProvider();
                    break;
                case SymmetricAlgorithmType.Custom: // added support for custom implementations of SymmetricAlgorithm for maximum flexibility
                {
                    if (config.CustomSymmetricAlgorithm == null)
                    { throw new InvalidOperationException("If SymmetricAlgorithmType.Custom is specified, CustomSymmetricAlgorithm cannot be null."); }

                    _algorithm = config.CustomSymmetricAlgorithm;
                }
                    break;
                default: throw new ArgumentException("The value '" + config.AlgorithmType + "' is not a valid SymmetricAlgorithmType.");
            }

            if (_algorithm != null)
            {
                _algorithm.Mode = config.CipherMode;
                _algorithm.Padding = config.PaddingMode;    
            }
        }

        /// <summary>
        /// Encrypts an array of bytes.
        /// </summary>
        /// <param name="input">Plaintext bytes to encrypt.</param>
        /// <param name="key">Encryption key bytes.</param>
        /// <param name="iv">Encryption IV bytes.</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] input, byte[] key, byte[] iv)
        {
            return PerformByteTransformation(input, key, iv, CryptoDirectionType.Encrypt);
        }

        /// <summary>
        /// Encrypts an array of bytes.
        /// </summary>
        /// <param name="input">Plaintext bytes to encrypt.</param>
        /// <param name="key">Encryption key string.</param>
        /// <param name="iv">Encryption IV string.</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] input, string key, string iv)
        {
            byte[] keyBytes = null;
            byte[] ivBytes = null;

            // Only check for transforms for the key and iv if we are actually using an encryption algorithm
            if (_algorithm != null)
            {
                if (_config.KeyTransform == null)
                { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a KeyTransform defined."); }
                if (_config.IvTransform == null)
                { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have an IVTransform defined."); }
                
                keyBytes = _config.KeyTransform.GetBytes(key);
                ivBytes = _config.IvTransform.GetBytes(iv);
            }

            return Encrypt(input, keyBytes, ivBytes);
        }

        /// <summary>
        /// Encrypts a string.
        /// </summary>
        /// <param name="input">String to encrypt.</param>
        /// <param name="key">Encryption key bytes.</param>
        /// <param name="iv">Encryption IV bytes.</param>
        /// <returns></returns>
        public string EncryptString(string input, byte[] key, byte[] iv)
        {
            if (_config.PlainTextTransform == null)
            { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a PlainTextTransform defined."); }
            if (_config.CipherTextTransform == null)
            { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a CipherTextTransform defined."); }

            var inputBytes = _config.PlainTextTransform.GetBytes(input);
            var outputBytes = Encrypt(inputBytes, key, iv);
            
            return _config.CipherTextTransform.GetString(outputBytes);
        }

        /// <summary>
        /// Encrypts a string.
        /// </summary>
        /// <param name="input">String to encrypt.</param>
        /// <param name="key">Encryption key string.</param>
        /// <param name="iv">Encryption IV string.</param>
        /// <returns></returns>
        public string EncryptString(string input, string key, string iv)
        {
            if (_config.PlainTextTransform == null)
            { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a PlainTextTransform defined."); }
            if (_config.CipherTextTransform == null)
            { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a CipherTextTransform defined."); }

            var inputBytes = _config.PlainTextTransform.GetBytes(input);
            var outputBytes = Encrypt(inputBytes, key, iv);

            return _config.CipherTextTransform.GetString(outputBytes);
        }

        /// <summary>
        /// Decrypts an array of bytes.
        /// </summary>
        /// <param name="input">Ciphertext bytes to decrypt.</param>
        /// <param name="key">Decryption key bytes.</param>
        /// <param name="iv">Decryption IV bytes.</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] input, byte[] key, byte[] iv)
        {
            return PerformByteTransformation(input, key, iv, CryptoDirectionType.Decrypt);
        }

        /// <summary>
        /// Decrypts an array of bytes.
        /// </summary>
        /// <param name="input">Ciphertext bytes to decrypt.</param>
        /// <param name="key">Decryption key string.</param>
        /// <param name="iv">Decryption IV string.</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] input, string key, string iv)
        {
            byte[] keyBytes = null;
            byte[] ivBytes = null;

            // Only check for transforms for the key and iv if we are actually using an encryption algorithm
            if (_algorithm != null)
            {
                if (_config.KeyTransform == null)
                { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a KeyTransform defined."); }
                if (_config.IvTransform == null)
                { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have an IVTransform defined."); }

                keyBytes = _config.KeyTransform.GetBytes(key);
                ivBytes = _config.IvTransform.GetBytes(iv);
            }

            return Decrypt(input, keyBytes, ivBytes);
        }

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="input">String to decrypt.</param>
        /// <param name="key">Decryption key bytes.</param>
        /// <param name="iv">Decryption IV bytes.</param>
        /// <returns></returns>
        public string DecryptString(string input, byte[] key, byte[] iv)
        {
            if (_config.PlainTextTransform == null)
            { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a PlainTextTransform defined."); }
            if (_config.CipherTextTransform == null)
            { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a CipherTextTransform defined."); }

            var inputBytes = _config.CipherTextTransform.GetBytes(input);
            var outputBytes = Decrypt(inputBytes, key, iv);

            return _config.PlainTextTransform.GetString(outputBytes);
        }

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="input">String to decrypt.</param>
        /// <param name="key">Decryption key string.</param>
        /// <param name="iv">Decryption IV string.</param>
        /// <returns></returns>
        public string DecryptString(string input, string key, string iv)
        {
            if (_config.PlainTextTransform == null)
            { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a PlainTextTransform defined."); }
            if (_config.CipherTextTransform == null)
            { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a CipherTextTransform defined."); }

            var inputBytes = _config.CipherTextTransform.GetBytes(input);
            var outputBytes = Decrypt(inputBytes, key, iv);

            return _config.PlainTextTransform.GetString(outputBytes);
        }

        /// <summary>
        /// Generates a random key byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] GenerateKey()
        {
            if (_algorithm == null)
            { throw new InvalidOperationException("No algorithm was defined because the SymmetricEncryptionConfiguration specified SymmetricAlgorithmType.None."); }

            _algorithm.GenerateKey();

            return _algorithm.Key;
        }

        /// <summary>
        /// Generates a random key string.
        /// </summary>
        /// <returns></returns>
        public string GenerateKeyString()
        {
            if (_config.KeyTransform == null)
            { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have a KeyTransform defined."); }

            return _config.KeyTransform.GetString(GenerateKey());
        }

        /// <summary>
        /// Generates a random IV byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] GenerateIv()
        {
            if (_algorithm == null)
            { throw new InvalidOperationException("No algorithm was defined because the SymmetricEncryptionConfiguration specified SymmetricAlgorithmType.None."); }

            _algorithm.GenerateIV();
            
            return _algorithm.IV;
        }

        /// <summary>
        /// Generates a random IV string.
        /// </summary>
        /// <returns></returns>
        public string GenerateIvString()
        {
            if (_config.IvTransform == null)
            { throw new InvalidOperationException("The SymmetricEncryptionConfiguration does not have an IVTransform defined."); }

            return _config.IvTransform.GetString(GenerateIv());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs the specified cryptographic operation with the specified settings.
        /// </summary>
        /// <param name="input">Input bytes to transform.</param>
        /// <param name="key">Key bytes to use for the transform.</param>
        /// <param name="iv">IV bytes to use for the transform.</param>
        /// <param name="cryptoDirection">One of the Hyper.Cryptography.SymmetricEncryptionService.CryptoDirectionType values.</param>
        /// <returns></returns>
        private byte[] PerformByteTransformation(byte[] input, byte[] key, byte[] iv, CryptoDirectionType cryptoDirection)
        {
            // No encryption service, so return input
            if (_algorithm == null)
            { return input; }

            // Set the keys
            _algorithm.Key = key;
            _algorithm.IV = iv;

            ICryptoTransform transformFunction;

            // Create the crypto stream object
            switch (cryptoDirection)
            {
                case CryptoDirectionType.Encrypt:
                    {
                        transformFunction = _algorithm.CreateEncryptor();
                    } break;
                case CryptoDirectionType.Decrypt:
                    {
                        transformFunction = _algorithm.CreateDecryptor();
                    } break;
                default: throw new InvalidOperationException("The value '" + cryptoDirection + "' is not a valid CryptoDirectionType.");
            }

            // Write out the transformed bytes
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, transformFunction, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(input, 0, input.Length);
                    cryptoStream.FlushFinalBlock();
                }
                
                return memoryStream.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// Indicates whether we are encrypting or decrypting.
        /// </summary>
        private enum CryptoDirectionType
        {
            Encrypt = 0,
            Decrypt = 1
        }
    }

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
