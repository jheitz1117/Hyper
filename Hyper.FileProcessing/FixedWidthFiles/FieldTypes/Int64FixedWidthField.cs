namespace Hyper.FileProcessing.FixedWidthFiles
{
    public class Int64FixedWidthField : NumericFixedWidthField
    {
        public long Value { get; set; }

        public Int64FixedWidthField() { }
        public Int64FixedWidthField(long value) : this() { Value = value; }
        public Int64FixedWidthField(long value, int length) : this(value) { Length = length; }

        protected override object GetValue()
        {
            return Value;
        }
    }
}
