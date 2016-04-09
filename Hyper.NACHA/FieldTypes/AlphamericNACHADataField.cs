using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class AlphamericNachaDataField : CustomFixedWidthField<string>
    {
        public AlphamericNachaDataField() { }

        public AlphamericNachaDataField(string value)
            : this()
        {
            Value = value;
        }

        public AlphamericNachaDataField(string value, int length)
            : this(value)
        {
            Length = length;
        }

        public AlphamericNachaDataField(string value, int length, CustomFixedWidthFormatDelegate<string> customFormat)
            : this(value, length)
        {
            Format = customFormat;
        }
    }
}
