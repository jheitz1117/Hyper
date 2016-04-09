using System.Collections.Generic;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NachaFillerRecord : NachaRecord
    {
        public override NachaRecordType RecordTypeCode => ((EnumeratedFixedWidthField<NachaRecordType>) Fields[0]).Value;

        public NachaFillerRecord()
        {
            // Define fields along with their type and length
            Fields = new List<IFixedWidthField>() {
                new EnumeratedFixedWidthField<NachaRecordType>(NachaRecordType.FileControl, 1),
                new AlphamericNachaDataField("", Length, FormatFillerRecord)
            };
        }

        private string FormatFillerRecord(string input, int fieldLength)
        {
            return new string('9', Length);
        }
    }
}
