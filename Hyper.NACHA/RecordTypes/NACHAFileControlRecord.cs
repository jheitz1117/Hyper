using System;
using System.Collections.Generic;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NachaFileControlRecord : NachaRecord
    {
        public override NachaRecordType RecordTypeCode => ((EnumeratedFixedWidthField<NachaRecordType>) Fields[0]).Value;

        public long BatchCount
        {
            get
            {
                return ((NumericNachaDataField) Fields[1]).Value;
            }
            set
            {
                ((NumericNachaDataField) Fields[1]).Value = value;
            }
        }

        public long BlockCount
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

        public long EntryAddendaCount
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

        public long EntryHash
        {
            get
            {
                return ((NumericNachaDataField) Fields[4]).Value;
            }
            set
            {
                ((NumericNachaDataField) Fields[4]).Value = value;
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
                ((NumericNachaDataField) Fields[5]).Value = (long)Math.Round(value * 100, 0);
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
                ((NumericNachaDataField) Fields[6]).Value = (long)Math.Round(value * 100, 0);
            }
        }

        public NachaFileControlRecord()
        {
            // Define fields along with their type and length
            Fields = new List<IFixedWidthField>() {
                new EnumeratedFixedWidthField<NachaRecordType>(NachaRecordType.FileControl, 1), // Record Type Code
                new NumericNachaDataField(0, 6), // Batch Count - Per NACHA docs, value must be equal to the number of CompanyBatchControl (8) records
                new NumericNachaDataField(0, 6), // Block Count - Per NACHA docs, value must be equal to the total number of blocks in the file, including the file header and file control records
                new NumericNachaDataField(0, 8), // Entry/Addenda Count - Per NACHA docs, value must be equal to the total number of EntryDetail (6) and Addenda (7) records
                new NumericNachaDataField(0, 10), // Entry Hash - Per NACHA docs, value must be equal to the total number of Receiving Depository Financial Institutions in each EntryDetail (6) record. If sum is more than 10 digits, truncate the leftmost digits
                new NumericNachaDataField(0, 12), // Total Debit Entry Dollar Amount in File - Total of all Debit amounts in CompanyBatchControl (8) records
                new NumericNachaDataField(0, 12), // Total Credit Entry Dollar Amount in File - Total of all Credit amounts in CompanyBatchControl (8) records
                new AlphabeticNachaDataField("", 39) // Reserved - Per NACHA docs, filled with blanks
            };
        }
    }
}
