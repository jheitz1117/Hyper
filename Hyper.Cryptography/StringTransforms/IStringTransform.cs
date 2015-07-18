namespace Hyper.Cryptography
{
    public interface IStringTransform
    {
        string GetString(byte[] input);
        byte[] GetBytes(string input);
    }
}
