using System;
using System.Collections.Generic;
using System.Text;

namespace Hyper.NACHA
{
    public class NACHAFile
    {
        /// <summary>
        /// The blocking factor determines the number of records in one block in the NACHA file. The NACHA file must contain only whole blocks, so if the
        /// number of records in the NACHA file is not a multiple of the blocking factor, the remainder of the final block of the file must be filled with
        /// records consisting of 9's
        /// </summary>
        public const int BlockingFactor = 10; // Per NACHA docs, all files have a blocking factor of 10
        public const int FormatCode = 1; // Per NACHA docs, all files have a format code of 1

        public string ImmediateDestination
        {
            get
            {
                return FileHeader.ImmediateDestination;
            }
            set
            {
                FileHeader.ImmediateDestination = value;
            }
        }

        public string ImmediateDestinationName
        {
            get
            {
                return FileHeader.ImmediateDestinationName;
            }
            set
            {
                FileHeader.ImmediateDestinationName = value;
            }
        }

        public string ImmediateOrigin
        {
            get
            {
                return FileHeader.ImmediateOrigin;
            }
            set
            {
                FileHeader.ImmediateOrigin = value;
            }
        }

        public string ImmediateOriginName
        {
            get
            {
                return FileHeader.ImmediateOriginName;
            }
            set
            {
                FileHeader.ImmediateOriginName = value;
            }
        }

        private readonly NACHAFileHeaderRecord FileHeader;
        private readonly NACHAFileControlRecord FileControl;
        private readonly List<NACHACompanyBatch> CompanyBatches = new List<NACHACompanyBatch>();

        public NACHAFile()
            : this(DateTime.Now) { }
        public NACHAFile(DateTime fileCreateDate)
        {
            FileHeader = new NACHAFileHeaderRecord(BlockingFactor, FormatCode, fileCreateDate);
            FileControl = new NACHAFileControlRecord();
        }

        public void AddCompanyBatch(NACHACompanyBatch batch)
        {
            CompanyBatches.Add(batch);
        }
        
        public void RemoveCompanyBatch(NACHACompanyBatch batch)
        {
            CompanyBatches.Remove(batch);
        }

        public override string ToString()
        {
            // Collapse our records into a single list
            var records = new List<NACHARecord>();
            records.Add(FileHeader);

            long batchCount = 0;
            foreach (var companyBatch in CompanyBatches)
            {
                // Increment first, then assign
                companyBatch.BatchNumber = ++batchCount;

                // Add the batch
                records.AddRange(companyBatch.GetBatchRecords());
            }

            FileControl.BatchCount = CompanyBatches.Count;

            records.Add(FileControl);

            // Fill final block with filler records if necessary
            int fillerRecordCount = (BlockingFactor - (records.Count % BlockingFactor)) % BlockingFactor;
            for (int i = 0; i < fillerRecordCount; i++)
            {
                records.Add(new NACHAFillerRecord());
            }

            var builder = new StringBuilder();

            FileControl.BlockCount = (long)Math.Ceiling((double)records.Count / BlockingFactor);

            // Reset composite fields
            FileControl.EntryAddendaCount = 0;
            FileControl.EntryHash = 0;

            foreach (var record in records)
            {
                switch (record.RecordTypeCode)
                {
                    case NACHARecordType.EntryDetail:
                        var entry = record as NACHAEntryDetailRecord;
                        FileControl.EntryAddendaCount++;
                        FileControl.EntryHash += entry.ReceivingDFIID;
                        break;
                    case NACHARecordType.Addenda:
                        FileControl.EntryAddendaCount++;
                        break;
                    case NACHARecordType.CompanyBatchControl:
                        var batchControl = record as NACHACompanyBatchControlRecord;
                        FileControl.TotalDebitAmount += batchControl.TotalDebitAmount; // Will only be populated after a call to GetBatchRecords()
                        FileControl.TotalCreditAmount += batchControl.TotalCreditAmount; // Will only be populated after a call to GetBatchRecords()
                        break;
                }

                // Fix the entry hash if it's too long. We want the right-most 10 digits, so can just take the entry hash mod 10000000000 (1 followed by 10 zeros)
                if (FileControl.EntryHash > 9999999999)
                {
                    FileControl.EntryHash = FileControl.EntryHash % 10000000000;
                }

                builder.AppendLine(record.ToString());
            }

            // DO NOT TRIM THIS RESULT
            // If there are just enough records such that no filler records are needed, then the final line will have 39 trailing spaces, which are required by the NACHA file format.
            // If we trim the result in that case, those 39 spaces will be removed and the file will be invalid
            return builder.ToString();
        }
    }
}
