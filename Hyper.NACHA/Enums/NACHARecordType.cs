namespace Hyper.NACHA
{
    public enum NACHARecordType
    {
        FileHeader = 1,
        CompanyBatchHeader = 5,
        EntryDetail = 6,
        Addenda = 7,
        CompanyBatchControl = 8,
        FileControl = 9
    }
}
