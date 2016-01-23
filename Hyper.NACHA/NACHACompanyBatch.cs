using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

namespace Hyper.NACHA
{
    public class NachaCompanyBatch
    {
        /// <summary>
        /// Defines the type of entries contained in the batch.
        /// </summary>
        public NachaServiceClassCodeType ServiceClassCode
        {
            get
            {
                return _companyBatchHeader.ServiceClassCode;
            }
            set
            {
                _companyBatchHeader.ServiceClassCode = value;
                _companyBatchControl.ServiceClassCode = value;
            }
        }

        public string CompanyName
        {
            get
            {
                return _companyBatchHeader.CompanyName;
            }
            set
            {
                _companyBatchHeader.CompanyName = value;
            }
        }

        public string CompanyDiscretionaryData
        {
            get
            {
                return _companyBatchHeader.CompanyDiscretionaryData;
            }
            set
            {
                _companyBatchHeader.CompanyDiscretionaryData = value;
            }
        }

        public string CompanyId
        {
            get
            {
                return _companyBatchHeader.CompanyId;
            }
            set
            {
                _companyBatchHeader.CompanyId = value;
                _companyBatchControl.CompanyId = value;
            }
        }

        public string BatchDescription
        {
            get
            {
                return _companyBatchHeader.BatchDescription;
            }
            set
            {
                _companyBatchHeader.BatchDescription = value;
            }
        }

        public string CompanyDescriptiveDate
        {
            get
            {
                return _companyBatchHeader.CompanyDescriptiveDate;
            }
            set
            {
                _companyBatchHeader.CompanyDescriptiveDate = value;
            }
        }

        public DateTime EffectiveEntryDate
        {
            get
            {
                return _companyBatchHeader.EffectiveEntryDate;
            }
            set
            {
                _companyBatchHeader.EffectiveEntryDate = value;
            }
        }

        /// <summary>
        /// Routing number of the originating bank
        /// </summary>
        public string OriginatingRoutingNumber
        {
            get { return _originatingRoutingNumber; }
            set
            {
                // null check, pad on the left with zeros to 9 places, truncate to 9 places
                var input = (value ?? "").Trim().PadLeft(9, '0').Substring(0, 9);

                // ensure 9 consecutive digits
                if (Regex.IsMatch(input, @"^\d{9}$"))
                {
                    OriginatingDfiId = Convert.ToInt64(input.Substring(0, 8));
                    _originatingRoutingNumber = value;
                }
                else
                {
                    throw new ArgumentException("The value '" + value + "' is not a valid routing number.");
                }
            }
        } private string _originatingRoutingNumber = "111111118";

        public long OriginatingDfiId
        {
            get { return _companyBatchHeader.OriginatingDfiId; }
            set
            {
                _companyBatchHeader.OriginatingDfiId = value;
                _companyBatchControl.OriginatingDfiId = value;
            }
        }

        public long BatchNumber
        {
            get { return _companyBatchHeader.BatchNumber; }
            set
            {
                _companyBatchHeader.BatchNumber = value;
                _companyBatchControl.BatchNumber = value;
            }
        }

        private readonly NachaCompanyBatchHeaderRecord _companyBatchHeader;
        private readonly NachaCompanyBatchControlRecord _companyBatchControl;
        private readonly List<NachaEntryDetailRecord> _entries = new List<NachaEntryDetailRecord>();

        public NachaCompanyBatch()
        {
            _companyBatchHeader = new NachaCompanyBatchHeaderRecord();
            _companyBatchControl = new NachaCompanyBatchControlRecord();
        }
        public NachaCompanyBatch(long batchNumber)
            : this()
        {
            BatchNumber = batchNumber;
        }
        public NachaCompanyBatch(long batchNumber, DateTime effectiveEntryDate)
            : this(batchNumber)
        {
            EffectiveEntryDate = effectiveEntryDate;
        }

        public void AddEntry(NachaEntryDetailRecord entry)
        {
            _entries.Add(entry);
        }

        public void RemoveEntry(NachaEntryDetailRecord entry)
        {
            _entries.Remove(entry);
        }

        internal List<NachaRecord> GetBatchRecords()
        {
            var records = new List<NachaRecord> {_companyBatchHeader};

            var hasCredits = false;
            var hasDebits = false;

            // Reset composite fields
            _companyBatchControl.EntryHash = 0;
            _companyBatchControl.TotalCreditAmount = 0;
            _companyBatchControl.TotalDebitAmount = 0;

            foreach (var entry in _entries)
            {
                // Update the entry hash
                _companyBatchControl.EntryHash += entry.ReceivingDfiId;

                if (entry.IsCredit)
                {
                    hasCredits = true;
                    _companyBatchControl.TotalCreditAmount += entry.Amount;
                }
                else
                {
                    hasDebits = true;
                    _companyBatchControl.TotalDebitAmount += entry.Amount;
                }

                records.Add(entry);
            }

            if (hasCredits && hasDebits)
                ServiceClassCode = NachaServiceClassCodeType.DebitsAndCredits;
            else if (hasCredits)
                ServiceClassCode = NachaServiceClassCodeType.CreditsOnly;
            else if (hasDebits)
                ServiceClassCode = NachaServiceClassCodeType.DebitsOnly;

            // Fix the entry hash if it's too long. We want the right-most 10 digits, so can just take the entry hash mod 10000000000 (1 followed by 10 zeros)
            if (_companyBatchControl.EntryHash > 9999999999)
                _companyBatchControl.EntryHash = _companyBatchControl.EntryHash % 10000000000;

            _companyBatchControl.EntryAddendaCount = _entries.Count;

            records.Add(_companyBatchControl);

            return records;
        }

        internal static string FormatCompanyId(string input, int fieldLength)
        {
            return (input ?? "123456789").PadLeft(fieldLength, '1');
        }
    }
}
