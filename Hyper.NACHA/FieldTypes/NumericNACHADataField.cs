using System;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NumericNachaDataField : Int64FixedWidthField
    {
        public NumericNachaDataField() { }

        public NumericNachaDataField(long value)
        {
            Value = value;
        }

        public NumericNachaDataField(long value, int length)
            : this(value)
        {
            Length = length;
        }

        protected override object GetValue()
        {
            // Per NACHA docs, numeric values must be unsigned
            return Math.Abs(Value);
        }
    }
}
