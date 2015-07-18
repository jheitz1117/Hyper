using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    public abstract class NACHARecord : FixedWidthRecordBase
    {
        public override sealed int Length { get { return 94; } } // Per NACHA docs, all records are 94 characters long
        public abstract NACHARecordType RecordTypeCode { get; }
    }
}
