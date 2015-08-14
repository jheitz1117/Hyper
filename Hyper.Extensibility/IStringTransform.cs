namespace Hyper.Extensibility.Cryptography
{
    /// <summary>
    /// Defines methods to transform <see cref="string"/> objects into <see cref="byte"/> arrays and vice versa.
    /// </summary>
    public interface IStringTransform
    {
        /// <summary>
        /// Transforms the specified <see cref="byte"/> array into a <see cref="string"/> representation.
        /// </summary>
        /// <param name="input">The <see cref="byte"/> array to transform.</param>
        /// <returns></returns>
        string GetString(byte[] input);

        /// <summary>
        /// Transforms the specified <see cref="string"/> into a <see cref="byte"/> array.
        /// </summary>
        /// <param name="input">The <see cref="string"/> to transform.</param>
        /// <returns></returns>
        byte[] GetBytes(string input);
    }
}
