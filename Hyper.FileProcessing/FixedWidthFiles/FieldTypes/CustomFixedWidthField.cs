namespace Hyper.FileProcessing.FixedWidthFiles
{
    public delegate object CustomFixedWidthFormatDelegate<in T>(T input, int length);

    public class CustomFixedWidthField<T> : FixedWidthFieldBase
    {
        public T Value { get; set; }
        public CustomFixedWidthFormatDelegate<T> Format { get; set; }

        public CustomFixedWidthField() { }
        public CustomFixedWidthField(T value) : this() { Value = value; }
        public CustomFixedWidthField(T value, int length) : this(value) { Length = length; }
        public CustomFixedWidthField(T value, int length, char paddingChar) : this(value, length) { PaddingChar = paddingChar; }
        public CustomFixedWidthField(T value, int length, CustomFixedWidthFormatDelegate<T> format) : this(value, length) { Format = format; }
        public CustomFixedWidthField(T value, int length, char paddingChar, CustomFixedWidthFormatDelegate<T> format) : this(value, length, paddingChar) { Format = format; }

        protected override object GetValue()
        {
            object result = Value;
            
            if (Format != null)
            {
                result = Format(Value, Length);
            }

            return result;
        }
    }
}
