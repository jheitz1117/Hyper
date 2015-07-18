using System;
using System.Collections.Generic;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NACHACompanyBatchControlRecord : NACHARecord
    {
        public override NACHARecordType RecordTypeCode
        {
            get { return (Fields[0] as EnumeratedFixedWidthField<NACHARecordType>).Value; }
        }

        /// <summary>
        /// Defines the type of entries contained in the batch.
        /// </summary>
        public NACHAServiceClassCodeType ServiceClassCode
        {
            get
            {
                return (Fields[1] as EnumeratedFixedWidthField<NACHAServiceClassCodeType>).Value;
            }
            set
            {
                (Fields[1] as EnumeratedFixedWidthField<NACHAServiceClassCodeType>).Value = value;
            }
        }

        public long EntryAddendaCount
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

        public long EntryHash
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
                (Fields[4] as NumericNACHADataField).Value = (long)Math.Round(value * 100, 0);
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
                (Fields[5] as NumericNACHADataField).Value = (long)Math.Round(value * 100, 0);
            }
        }

        /// <summary>
        /// ID of the company originating the transaction. Usually assigned by the receiving bank.
        /// </summary>
        public string CompanyID
        {
            get
            {
                return (Fields[6] as AlphamericNACHADataField).Value;
            }
            set
            {
                (Fields[6] as AlphamericNACHADataField).Value = value;
            }
        }

        /// <summary>
        /// The first 8 digits of the routing number of the originating bank
        /// </summary>
        public long OriginatingDFIID
        {
            get
            {
                return (Fields[9] as NumericNACHADataField).Value;
            }
            set
            {
                (Fields[9] as NumericNACHADataField).Value = value;
            }
        }

        public long BatchNumber
        {
            get
            {
                return (Fields[10] as NumericNACHADataField).Value;
            }
            set
            {
                (Fields[10] as NumericNACHADataField).Value = value;
            }
        }

        public NACHACompanyBatchControlRecord()
        {
            // Define fields along with their type and length
            Fields = new List<IFixedWidthField>() {
                new EnumeratedFixedWidthField<NACHARecordType>(NACHARecordType.CompanyBatchControl, 1), // Record Type Code
                new EnumeratedFixedWidthField<NACHAServiceClassCodeType>(NACHAServiceClassCodeType.DebitsAndCredits, 3), // Service Class Code - Available values are 200=Credits & Debits, 220=Credits Only, 225=Debits Only
                new NumericNACHADataField(0, 6), // Entry/Addenda Count - Per NACHA docs, value must be equal to the total number of EntryDetail (6) and Addenda (7) records
                new NumericNACHADataField(0, 10), // Entry Hash - Per NACHA docs, value must be equal to the total number of Receiving Depository Financial Institutions in each EntryDetail (6) record. If sum is more than 10 digits, truncate the leftmost digits
                new NumericNACHADataField(0, 12), // Total Debit Entry Dollar Amount in File - Total of all Debit amounts in CompanyBatchControl (8) records
                new NumericNACHADataField(0, 12), // Total Credit Entry Dollar Amount in File - Total of all Credit amounts in CompanyBatchControl (8) records
                new AlphamericNACHADataField("", 10, NACHACompanyBatch.FormatCompanyID), // Company Identification
                new AlphabeticNACHADataField("", 19), // Message Authentication Code - Per NACHA docs, optional. Leaving blank.
                new AlphabeticNACHADataField("", 6), // Reserved - Per NACHA docs, leaving blank.
                new NumericNACHADataField(11111111, 8), // Originating DFI ID - First 8 digits of the originating bank's routing number
                new NumericNACHADataField(0, 7) // Batch Number - Per NACHA docs, must be an ascending sequence number counting the batches in the file
            };
        }

        public NACHACompanyBatchControlRecord(long batchNumber)
            : this()
        {
            BatchNumber = batchNumber;
        }
    }
}
