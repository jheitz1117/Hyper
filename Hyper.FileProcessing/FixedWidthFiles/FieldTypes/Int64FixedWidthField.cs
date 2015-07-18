using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyper.FileProcessing.FixedWidthFiles
{
    public class Int64FixedWidthField : NumericFixedWidthField
    {
        public long Value { get; set; }

        public Int64FixedWidthField() { }
        public Int64FixedWidthField(long value) : this() { this.Value = value; }
        public Int64FixedWidthField(long value, int length) : this(value) { this.Length = length; }

        protected override object GetValue()
        {
            return Value;
        }
    }
}
