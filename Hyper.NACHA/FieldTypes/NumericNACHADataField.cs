using System;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NumericNACHADataField : Int64FixedWidthField
    {
        public NumericNACHADataField() { }
        public NumericNACHADataField(long value)
        {
            Value = value;
        }
        public NumericNACHADataField(long value, int length)
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
