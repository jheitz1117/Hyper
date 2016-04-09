using System;
using System.Collections.Generic;
using System.Text;

namespace Hyper.NACHA
{
    public class NachaFile
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
                return _fileHeader.ImmediateDestination;
            }
            set
            {
                _fileHeader.ImmediateDestination = value;
            }
        }

        public string ImmediateDestinationName
        {
            get
            {
                return _fileHeader.ImmediateDestinationName;
            }
            set
            {
                _fileHeader.ImmediateDestinationName = value;
            }
        }

        public string ImmediateOrigin
        {
            get
            {
                return _fileHeader.ImmediateOrigin;
            }
            set
            {
                _fileHeader.ImmediateOrigin = value;
            }
        }

        public string ImmediateOriginName
        {
            get
            {
                return _fileHeader.ImmediateOriginName;
            }
            set
            {
                _fileHeader.ImmediateOriginName = value;
            }
        }

        private readonly NachaFileHeaderRecord _fileHeader;
        private readonly NachaFileControlRecord _fileControl;
        private readonly List<NachaCompanyBatch> _companyBatches = new List<NachaCompanyBatch>();

        public NachaFile()
            : this(DateTime.Now) { }

        public NachaFile(DateTime fileCreateDate)
        {
            _fileHeader = new NachaFileHeaderRecord(BlockingFactor, FormatCode, fileCreateDate);
            _fileControl = new NachaFileControlRecord();
        }

        public void AddCompanyBatch(NachaCompanyBatch batch)
        {
            _companyBatches.Add(batch);
        }
        
        public void RemoveCompanyBatch(NachaCompanyBatch batch)
        {
            _companyBatches.Remove(batch);
        }

        public override string ToString()
        {
            // Collapse our records into a single list
            var records = new List<NachaRecord> {_fileHeader};

            long batchCount = 0;
            foreach (var companyBatch in _companyBatches)
            {
                // Increment first, then assign
                companyBatch.BatchNumber = ++batchCount;

                // Add the batch
                records.AddRange(companyBatch.GetBatchRecords());
            }

            _fileControl.BatchCount = _companyBatches.Count;

            records.Add(_fileControl);

            // Fill final block with filler records if necessary
            var fillerRecordCount = (BlockingFactor - (records.Count % BlockingFactor)) % BlockingFactor;
            for (var i = 0; i < fillerRecordCount; i++)
            {
                records.Add(new NachaFillerRecord());
            }

            var builder = new StringBuilder();

            _fileControl.BlockCount = (long)Math.Ceiling((double)records.Count / BlockingFactor);

            // Reset composite fields
            _fileControl.EntryAddendaCount = 0;
            _fileControl.EntryHash = 0;

            foreach (var record in records)
            {
                switch (record.RecordTypeCode)
                {
                    case NachaRecordType.EntryDetail:
                        var entry = (NachaEntryDetailRecord)record;
                        _fileControl.EntryAddendaCount++;
                        _fileControl.EntryHash += entry.ReceivingDfiId;
                        break;
                    case NachaRecordType.Addenda:
                        _fileControl.EntryAddendaCount++;
                        break;
                    case NachaRecordType.CompanyBatchControl:
                        var batchControl = (NachaCompanyBatchControlRecord)record;
                        _fileControl.TotalDebitAmount += batchControl.TotalDebitAmount; // Will only be populated after a call to GetBatchRecords()
                        _fileControl.TotalCreditAmount += batchControl.TotalCreditAmount; // Will only be populated after a call to GetBatchRecords()
                        break;
                }

                // Fix the entry hash if it's too long. We want the right-most 10 digits, so can just take the entry hash mod 10000000000 (1 followed by 10 zeros)
                if (_fileControl.EntryHash > 9999999999)
                    _fileControl.EntryHash = _fileControl.EntryHash % 10000000000;

                builder.AppendLine(record.ToString());
            }

            // DO NOT TRIM THIS RESULT
            // If there are just enough records such that no filler records are needed, then the final line will have 39 trailing spaces, which are required by the NACHA file format.
            // If we trim the result in that case, those 39 spaces will be removed and the file will be invalid
            return builder.ToString();
        }
    }
}
