using System.Collections.Generic;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NACHAFillerRecord : NACHARecord
    {
        public override NACHARecordType RecordTypeCode
        {
            get { return (Fields[0] as EnumeratedFixedWidthField<NACHARecordType>).Value; }
        }

        public NACHAFillerRecord()
        {
            // Define fields along with their type and length
            Fields = new List<IFixedWidthField>() {
                new EnumeratedFixedWidthField<NACHARecordType>(NACHARecordType.FileControl, 1),
                new AlphamericNACHADataField("", Length, FormatFillerRecord)
            };
        }

        private string FormatFillerRecord(string input, int fieldLength)
        {
            return new string('9', Length);
        }
    }
}
