using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyper.FileProcessing.FixedWidthFiles
{
    public abstract class NumericFixedWidthField : FixedWidthFieldBase
    {
        public override FixedWidthJustifyType JustifyType
        {
            get
            {
                return FixedWidthJustifyType.Right;
            }
        }

        public override char PaddingChar
        {
            get
            {
                return '0';
            }
        }
    }
}
