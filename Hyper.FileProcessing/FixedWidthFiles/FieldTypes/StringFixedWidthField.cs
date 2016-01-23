namespace Hyper.FileProcessing.FixedWidthFiles
{
    public class StringFixedWidthField : FixedWidthFieldBase
    {
        public string Value { get; set; }

        public StringFixedWidthField() { }
        public StringFixedWidthField(string value) : this() { Value = value; }
        public StringFixedWidthField(string value, int length) : this(value) { Length = length; }

        protected override object GetValue()
        {
            return Value;
        }
    }
}
