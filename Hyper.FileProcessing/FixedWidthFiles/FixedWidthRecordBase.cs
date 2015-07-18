using System.Collections.Generic;
using System.Text;

namespace Hyper.FileProcessing.FixedWidthFiles
{
    public abstract class FixedWidthRecordBase
    {
        protected List<IFixedWidthField> Fields { get; set; }
        public abstract int Length { get; } // Delegate the record length to the subclasses
        public virtual FixedWidthJustifyType JustifyType { get { return FixedWidthJustifyType.Left; } }
        public virtual char PaddingChar { get { return ' '; } } // Default record-level padding is a space

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var field in Fields)
            {
                builder.Append(field.GetStringForFile());
            }

            // Whatever fields we include, ensure the record is justified correctly and is exactly as long as the Length property
            return FixedWidthHelper.JustifyString(builder.ToString(), Length, PaddingChar, JustifyType);
        }
    }
}
