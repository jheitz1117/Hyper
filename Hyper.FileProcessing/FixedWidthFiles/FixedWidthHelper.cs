using System;

namespace Hyper.FileProcessing.FixedWidthFiles
{
    public static class FixedWidthHelper
    {
        public static string JustifyString(string input, int length, char paddingChar, FixedWidthJustifyType justifyType)
        {
            var result = input ?? "";

            switch (justifyType)
            {
                case FixedWidthJustifyType.Left:
                    result = result.PadRight(length, paddingChar);
                    break;
                case FixedWidthJustifyType.Right:
                    result = result.PadLeft(length, paddingChar);
                    break;
                case FixedWidthJustifyType.None:
                    // Do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(justifyType), justifyType, null);
            }

            return result.Substring(0, Math.Min(result.Length, length));
        }
    }
}
