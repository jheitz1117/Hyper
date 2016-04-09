using System;
using System.Collections.Generic;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NachaCompanyBatchHeaderRecord : NachaRecord
    {
        public override NachaRecordType RecordTypeCode => ((EnumeratedFixedWidthField<NachaRecordType>) Fields[0]).Value;

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

        /// <summary>
        /// The name of the company
        /// </summary>
        public string CompanyName
        {
            get
            {
                return (Fields[2] as AlphamericNachaDataField)?.Value;
            }
            set
            {
                ((AlphamericNachaDataField) Fields[2]).Value = value;
            }
        }

        /// <summary>
        /// Optional field. Contains reference information for use by the originator.
        /// </summary>
        public string CompanyDiscretionaryData
        {
            get
            {
                return (Fields[3] as AlphamericNachaDataField)?.Value;
            }
            set
            {
                ((AlphamericNachaDataField) Fields[3]).Value = value;
            }
        }

        /// <summary>
        /// ID of the company originating the transaction. Usually assigned by the receiving bank.
        /// </summary>
        public string CompanyId
        {
            get
            {
                return (Fields[4] as AlphamericNachaDataField)?.Value;
            }
            set
            {
                ((AlphamericNachaDataField) Fields[4]).Value = value;
            }
        }

        /// <summary>
        /// Defines the type of ACH Entries contained in the batch. Available values are PPD=Prearranged Payments and Deposits, CCD=Cash Concentration or Disbursement.
        /// Use PPD to send money to individuals, and CCD to send money to businesses.
        /// </summary>
        public string StandardEntryClassCode
        {
            get
            {
                return (Fields[5] as AlphamericNachaDataField)?.Value;
            }
            set
            {
                ((AlphamericNachaDataField) Fields[5]).Value = value;
            }
        }

        /// <summary>
        /// Description of the batch (i.e. Payroll, Direct Deposit, Dividend, etc.)
        /// </summary>
        public string BatchDescription
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
        /// This field is used by the originator to provide a descriptive date for the receiver. This is solely for descriptive purposes and will not be used to calculate settlement or used for posting pourposes.
        /// </summary>
        public string CompanyDescriptiveDate
        {
            get
            {
                return (Fields[7] as AlphamericNachaDataField)?.Value;
            }
            set
            {
                ((AlphamericNachaDataField) Fields[7]).Value = value;
            }
        }

        private DateTime _effectiveEntryDate = DateTime.Now;
        public DateTime EffectiveEntryDate
        {
            get
            {
                return _effectiveEntryDate;
            }
            set
            {
                _effectiveEntryDate = value;
                ((AlphamericNachaDataField) Fields[8]).Value = value.ToString("yyMMdd");
            }
        }

        /// <summary>
        /// The first 8 digits of the routing number of the originating bank
        /// </summary>
        public long OriginatingDfiId
        {
            get
            {
                return ((NumericNachaDataField) Fields[11]).Value;
            }
            set
            {
                ((NumericNachaDataField) Fields[11]).Value = value;
            }
        }

        public long BatchNumber
        {
            get
            {
                return ((NumericNachaDataField) Fields[12]).Value;
            }
            set
            {
                ((NumericNachaDataField) Fields[12]).Value = value;
            }
        }

        public NachaCompanyBatchHeaderRecord()
        {
            // Define fields along with their type and length
            Fields = new List<IFixedWidthField>() {
                new EnumeratedFixedWidthField<NachaRecordType>(NachaRecordType.CompanyBatchHeader, 1), // Record Type Code
                new EnumeratedFixedWidthField<NachaServiceClassCodeType>(NachaServiceClassCodeType.DebitsAndCredits, 3), // Service Class Code - Available values are 200=Credits & Debits, 220=Credits Only, 225=Debits Only
                new AlphamericNachaDataField("", 16), // Company Name - Per NACHA docs, this should exactly match the name on the company's bank statement from the receiving bank
                new AlphamericNachaDataField("", 20), // Company Discretionary Data - Optional field. Leaving blank.
                new AlphamericNachaDataField("", 10, NachaCompanyBatch.FormatCompanyId), // Company ID - Assigned by the bank.
                new AlphamericNachaDataField("PPD", 3), // Standard Entry Class Code - Available values are PPD=Prearranged Payments and Deposits, CCD=Cash Concentration or Disbursement
                                                        // Because of the nature of Exigo's business model, we will assume PPD for all transactions
                new AlphamericNachaDataField("", 10), // Company Entry Description - Description of the transaction batch as a whole
                new AlphamericNachaDataField("", 6), // Company Descriptive Date - Optional field. Leaving blank.
                new AlphamericNachaDataField(_effectiveEntryDate.ToString("yyMMdd"), 6), // Effective Entry Date - Date on which the originator wishes for the entries to be settled
                new AlphamericNachaDataField("", 3), // Settlement Date (Julian) - Per NACHA docs, left blank
                new AlphabeticNachaDataField("1", 1), // Originator Status Code - Per NACHA docs, field must contain 1
                new NumericNachaDataField(11111111, 8), // Originating DFI ID - Per NACHA docs, contains the first 8 digits of the PNC ABA or Transit Routing Number
                new NumericNachaDataField(0, 7) // Batch Number - Per NACHA docs, must be an ascending sequence number counting the batches in the file
            };
        }

        public NachaCompanyBatchHeaderRecord(long batchNumber)
            : this()
        {
            BatchNumber = batchNumber;
        }
        
        public NachaCompanyBatchHeaderRecord(long batchNumber, DateTime effectiveEntryDate)
            : this(batchNumber)
        {
            EffectiveEntryDate = effectiveEntryDate;
        }
    }
}
