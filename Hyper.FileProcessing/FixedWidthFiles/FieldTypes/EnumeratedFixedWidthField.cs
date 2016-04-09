using System;

namespace Hyper.FileProcessing.FixedWidthFiles
{
    public class EnumeratedFixedWidthField<T> : NumericFixedWidthField where T : struct, IConvertible
    {
        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (!Enum.IsDefined(typeof(T), value))
                    throw new ArgumentOutOfRangeException("The value '" + value + "' is not a valid " + typeof(T) + ".");

                _value = value;
            }
        }
        
        public EnumeratedFixedWidthField()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type.");
        }

        public EnumeratedFixedWidthField(T value) : this() { Value = value; }
        public EnumeratedFixedWidthField(T value, int length) : this(value) { Length = length; }

        protected override object GetValue()
        {
            return Convert.ToInt64(Value);
        }
    }
}
