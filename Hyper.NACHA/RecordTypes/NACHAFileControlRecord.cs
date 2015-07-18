using System;
using System.Collections.Generic;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NACHAFileControlRecord : NACHARecord
    {
        public override NACHARecordType RecordTypeCode
        {
            get { return (Fields[0] as EnumeratedFixedWidthField<NACHARecordType>).Value; }
        }

        public long BatchCount
        {
            get
            {
                return (Fields[1] as NumericNACHADataField).Value;
            }
            set
            {
                (Fields[1] as NumericNACHADataField).Value = value;
            }
        }

        public long BlockCount
        {
            get
            {
                return (Fields[2] as NumericNACHADataField).Value;
            }
            set
            {
                (Fields[2] as NumericNACHADataField).Value = value;
            }
        }

        public long EntryAddendaCount
        {
            get
            {
                return (Fields[3] as NumericNACHADataField).Value;
            }
            set
            {
                (Fields[3] as NumericNACHADataField).Value = value;
            }
        }

        public long EntryHash
        {
            get
            {
                return (Fields[4] as NumericNACHADataField).Value;
            }
            set
            {
                (Fields[4] as NumericNACHADataField).Value = value;
            }
        }

        decimal _totalDebitAmount;
        public decimal TotalDebitAmount
        {
            get
            {
                return _totalDebitAmount;
            }
            set
            {
                _totalDebitAmount = value;
                (Fields[5] as NumericNACHADataField).Value = (long)Math.Round(value * 100, 0);
            }
        }

        decimal _totalCreditAmount;
        public decimal TotalCreditAmount
        {
            get
            {
                return _totalCreditAmount;
            }
            set
            {
                _totalCreditAmount = value;
                (Fields[6] as NumericNACHADataField).Value = (long)Math.Round(value * 100, 0);
            }
        }

        public NACHAFileControlRecord()
        {
            // Define fields along with their type and length
            Fields = new List<IFixedWidthField>() {
                new EnumeratedFixedWidthField<NACHARecordType>(NACHARecordType.FileControl, 1), // Record Type Code
                new NumericNACHADataField(0, 6), // Batch Count - Per NACHA docs, value must be equal to the number of CompanyBatchControl (8) records
                new NumericNACHADataField(0, 6), // Block Count - Per NACHA docs, value must be equal to the total number of blocks in the file, including the file header and file control records
                new NumericNACHADataField(0, 8), // Entry/Addenda Count - Per NACHA docs, value must be equal to the total number of EntryDetail (6) and Addenda (7) records
                new NumericNACHADataField(0, 10), // Entry Hash - Per NACHA docs, value must be equal to the total number of Receiving Depository Financial Institutions in each EntryDetail (6) record. If sum is more than 10 digits, truncate the leftmost digits
                new NumericNACHADataField(0, 12), // Total Debit Entry Dollar Amount in File - Total of all Debit amounts in CompanyBatchControl (8) records
                new NumericNACHADataField(0, 12), // Total Credit Entry Dollar Amount in File - Total of all Credit amounts in CompanyBatchControl (8) records
                new AlphabeticNACHADataField("", 39) // Reserved - Per NACHA docs, filled with blanks
            };
        }
    }
}
