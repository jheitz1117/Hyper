using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyper.FileProcessing.FixedWidthFiles
{
    public class StringFixedWidthField : FixedWidthFieldBase
    {
        public string Value { get; set; }

        public StringFixedWidthField() { }
        public StringFixedWidthField(string value) : this() { this.Value = value; }
        public StringFixedWidthField(string value, int length) : this(value) { this.Length = length; }

        protected override object GetValue()
        {
            return Value;
        }
    }
}
