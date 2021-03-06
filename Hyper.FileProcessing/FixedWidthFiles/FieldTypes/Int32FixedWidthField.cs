﻿namespace Hyper.FileProcessing.FixedWidthFiles
{
    public class Int32FixedWidthField : NumericFixedWidthField
    {
        public int Value { get; set; }

        public Int32FixedWidthField() { }
        public Int32FixedWidthField(int value) : this() { Value = value; }
        public Int32FixedWidthField(int value, int length) : this(value) { Length = length; }

        protected override object GetValue()
        {
            return Value;
        }
    }
}
