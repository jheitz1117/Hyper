using System;
using System.Collections.Generic;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NachaCompanyBatchControlRecord : NachaRecord
    {
        public override NachaRecordType RecordTypeCode => ((EnumeratedFixedWidthField<NachaRecordType>)Fields[0]).Value;

        /// <summary>
        /// Defines the type of entries contained in the batch.
        /// </summary>
        public NachaServiceClassCodeType ServiceClassCode
        {
            get
            {
                return ((EnumeratedFixedWidthField<NachaServiceClassCodeType>) Fields[1]).Value;
            }
            set
            {
                ((EnumeratedFixedWidthField<NachaServiceClassCodeType>) Fields[1]).Value = value;
            }
        }

        public long EntryAddendaCount
        {
            get
            {
                return ((NumericNachaDataField) Fields[2]).Value;
            }
            set
            {
                ((NumericNachaDataField) Fields[2]).Value = value;
            }
        }

        public long EntryHash
        {
            get
            {
                return ((NumericNachaDataField) Fields[3]).Value;
            }
            set
            {
                ((NumericNachaDataField) Fields[3]).Value = value;
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
                ((NumericNachaDataField) Fields[4]).Value = (long)Math.Round(value * 100, 0);
            }
        }

        private decimal _totalCreditAmount;
        public decimal TotalCreditAmount
        {
            get
            {
                return _totalCreditAmount;
            }
            set
            {
                _totalCreditAmount = value;
                ((NumericNachaDataField) Fields[5]).Value = (long)Math.Round(value * 100, 0);
            }
        }

        /// <summary>
        /// ID of the company originating the transaction. Usually assigned by the receiving bank.
        /// </summary>
        public string CompanyId
        {
            get
            {
                return (Fields[6] as AlphamericNachaDataField)?.Value;
            }
            set
            {
                ((AlphamericNachaDataField) Fields[6]).Value = value;
            }
        }

        /// <summary>
        /// The first 8 digits of the routing number of the originating bank
        /// </summary>
        public long OriginatingDfiId
        {
            get
            {
                return ((NumericNachaDataField) Fields[9]).Value;
            }
            set
            {
                ((NumericNachaDataField) Fields[9]).Value = value;
            }
        }

        public long BatchNumber
        {
            get
            {
                return ((NumericNachaDataField) Fields[10]).Value;
            }
            set
            {
                ((NumericNachaDataField) Fields[10]).Value = value;
            }
        }

        public NachaCompanyBatchControlRecord()
        {
            // Define fields along with their type and length
            Fields = new List<IFixedWidthField>() {
                new EnumeratedFixedWidthField<NachaRecordType>(NachaRecordType.CompanyBatchControl, 1), // Record Type Code
                new EnumeratedFixedWidthField<NachaServiceClassCodeType>(NachaServiceClassCodeType.DebitsAndCredits, 3), // Service Class Code - Available values are 200=Credits & Debits, 220=Credits Only, 225=Debits Only
                new NumericNachaDataField(0, 6), // Entry/Addenda Count - Per NACHA docs, value must be equal to the total number of EntryDetail (6) and Addenda (7) records
                new NumericNachaDataField(0, 10), // Entry Hash - Per NACHA docs, value must be equal to the total number of Receiving Depository Financial Institutions in each EntryDetail (6) record. If sum is more than 10 digits, truncate the leftmost digits
                new NumericNachaDataField(0, 12), // Total Debit Entry Dollar Amount in File - Total of all Debit amounts in CompanyBatchControl (8) records
                new NumericNachaDataField(0, 12), // Total Credit Entry Dollar Amount in File - Total of all Credit amounts in CompanyBatchControl (8) records
                new AlphamericNachaDataField("", 10, NachaCompanyBatch.FormatCompanyId), // Company Identification
                new AlphabeticNachaDataField("", 19), // Message Authentication Code - Per NACHA docs, optional. Leaving blank.
                new AlphabeticNachaDataField("", 6), // Reserved - Per NACHA docs, leaving blank.
                new NumericNachaDataField(11111111, 8), // Originating DFI ID - First 8 digits of the originating bank's routing number
                new NumericNachaDataField(0, 7) // Batch Number - Per NACHA docs, must be an ascending sequence number counting the batches in the file
            };
        }

        public NachaCompanyBatchControlRecord(long batchNumber)
            : this()
        {
            BatchNumber = batchNumber;
        }
    }
}
