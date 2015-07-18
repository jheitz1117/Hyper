using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class AlphamericNACHADataField : CustomFixedWidthField<string>
    {
        public AlphamericNACHADataField() { }
        public AlphamericNACHADataField(string value)
            : this()
        {
            Value = value;
        }
        public AlphamericNACHADataField(string value, int length)
            : this(value)
        {
            Length = length;
        }
        public AlphamericNACHADataField(string value, int length, CustomFixedWidthFormatDelegate<string> customFormat)
            : this(value, length)
        {
            Format = customFormat;
        }
    }
}
