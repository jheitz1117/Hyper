using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class AlphabeticNachaDataField : StringFixedWidthField
    {
        public AlphabeticNachaDataField() { }
        public AlphabeticNachaDataField(string value)
        {
            Value = value;
        }
        public AlphabeticNachaDataField(string value, int length)
            : this(value)
        {
            Length = length;
        }

        protected override object GetValue()
        {
            return Value ?? "";
        }
    }
}
