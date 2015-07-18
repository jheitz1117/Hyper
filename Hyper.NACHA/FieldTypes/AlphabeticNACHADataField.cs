using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class AlphabeticNACHADataField : StringFixedWidthField
    {
        public AlphabeticNACHADataField() { }
        public AlphabeticNACHADataField(string value)
        {
            Value = value;
        }
        public AlphabeticNACHADataField(string value, int length)
            : this(value)
        {
            Length = length;
        }

        protected override object GetValue()
        {
            return (Value ?? "");
        }
    }
}
