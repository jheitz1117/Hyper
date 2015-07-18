using System;
using System.Text;
using Hyper.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyper.Test
{
    [TestClass]
    public class CryptographyTest
    {
        private const string InputStringTest = "Test String";

        [TestMethod]
        public void NoEncryptionTest()
        {
            var encryption = new SymmetricEncryptionProvider(
                new SymmetricEncryptionConfiguration()
                {
                    AlgorithmType = SymmetricAlgorithmType.None,
                    PlainTextTransform = new Utf8StringTransform(),
                    CipherTextTransform = new Utf8StringTransform()
                }
            );

            const string inputString = InputStringTest;
            var outputString = encryption.EncryptString(inputString, (string)null, null);

            Assert.AreEqual(inputString, outputString, "Encrypted string was not equal to the input string.");
        }

        [TestMethod]
        public void NoEncryptionTransformTest()
        {
            var encryption = new SymmetricEncryptionProvider(
                new SymmetricEncryptionConfiguration()
                {
                    AlgorithmType = SymmetricAlgorithmType.None,
                    PlainTextTransform = new Utf8StringTransform(),
                    CipherTextTransform = new Base64StringTransform()
                }
            );

            const string inputString = InputStringTest;
            var encryptionOutput = encryption.EncryptString(inputString, (string)null, null);
            var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(inputString));

            Assert.AreEqual(base64String, encryptionOutput, "Encrypted string was not equal to the base64 hash of the input string.");
        }

        [TestMethod]
        public void EncryptDecryptStringRoundTripTest()
        {
            var encryption = new SymmetricEncryptionProvider(
                new SymmetricEncryptionConfiguration()
            );

            var key = encryption.GenerateKeyString();
            var iv = encryption.GenerateIVString();

            const string inputString = InputStringTest;
            var encryptedResult = encryption.EncryptString(inputString, key, iv);
            var decryptedResult = encryption.DecryptString(encryptedResult, key, iv);

            Assert.AreEqual(inputString, decryptedResult, "Decrypted result is not equal to the original string.");
        }

        [TestMethod]
        public void CustomKeyTransformRoundTripTest()
        {
            var encryption = new SymmetricEncryptionProvider(
                new SymmetricEncryptionConfiguration() {
                    KeyTransform = new CustomKeyStringTransform(),
                    IVTransform = new CustomIVStringTransform()
                }
            );

            const string key = "Test Key";
            const string iv = "Testing IV";

            const string inputString = InputStringTest;
            var encryptedResult = encryption.EncryptString(inputString, key, iv);
            var decryptedResult = encryption.DecryptString(encryptedResult, key, iv);

            Assert.AreEqual(inputString, decryptedResult, "Decrypted result is not equal to the original string.");
        }

        private class CustomKeyStringTransform : Utf8StringTransform, IStringTransform
        {
            string IStringTransform.GetString(byte[] input)
            {
                return base.GetString(input);
            }

            byte[] IStringTransform.GetBytes(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                { throw new ArgumentNullException("input"); }
                else
                    return base.GetBytes(input.PadLeft(32, 'X').Substring(0, 32));
            }
        }

        private class CustomIVStringTransform : Utf8StringTransform, IStringTransform
        {
            string IStringTransform.GetString(byte[] input)
            {
                return base.GetString(input);
            }

            byte[] IStringTransform.GetBytes(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                { throw new ArgumentNullException("input"); }
                else
                    return base.GetBytes(input.PadLeft(16, 'X').Substring(0, 16));
            }
        }
    }
}
