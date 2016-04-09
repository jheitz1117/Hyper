using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    public abstract class NachaRecord : FixedWidthRecordBase
    {
        public sealed override int Length => 94; // Per NACHA docs, all records are 94 characters long
        public abstract NachaRecordType RecordTypeCode { get; }
    }
}
