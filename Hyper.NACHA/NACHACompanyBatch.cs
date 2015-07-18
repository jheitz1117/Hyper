using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

namespace Hyper.NACHA
{
    public class NACHACompanyBatch
    {
        /// <summary>
        /// Defines the type of entries contained in the batch.
        /// </summary>
        public NACHAServiceClassCodeType ServiceClassCode
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

        public string CompanyID
        {
            get
            {
                return _companyBatchHeader.CompanyID;
            }
            set
            {
                _companyBatchHeader.CompanyID = value;
                _companyBatchControl.CompanyID = value;
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
                string input = (value ?? "").Trim().PadLeft(9, '0').Substring(0, 9);

                // ensure 9 consecutive digits
                if (Regex.IsMatch(input, @"^\d{9}$"))
                {
                    OriginatingDFIID = Convert.ToInt64(input.Substring(0, 8));
                    _originatingRoutingNumber = value;
                }
                else
                {
                    throw new ArgumentException("The value '" + value + "' is not a valid routing number.");
                }
            }
        } private string _originatingRoutingNumber = "111111118";

        public long OriginatingDFIID
        {
            get { return _companyBatchHeader.OriginatingDFIID; }
            set
            {
                _companyBatchHeader.OriginatingDFIID = value;
                _companyBatchControl.OriginatingDFIID = value;
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

        private readonly NACHACompanyBatchHeaderRecord _companyBatchHeader;
        private readonly NACHACompanyBatchControlRecord _companyBatchControl;
        private readonly List<NACHAEntryDetailRecord> _entries = new List<NACHAEntryDetailRecord>();

        public NACHACompanyBatch()
        {
            _companyBatchHeader = new NACHACompanyBatchHeaderRecord();
            _companyBatchControl = new NACHACompanyBatchControlRecord();
        }
        public NACHACompanyBatch(long batchNumber)
            : this()
        {
            BatchNumber = batchNumber;
        }
        public NACHACompanyBatch(long batchNumber, DateTime effectiveEntryDate)
            : this(batchNumber)
        {
            EffectiveEntryDate = effectiveEntryDate;
        }

        public void AddEntry(NACHAEntryDetailRecord entry)
        {
            _entries.Add(entry);
        }

        public void RemoveEntry(NACHAEntryDetailRecord entry)
        {
            _entries.Remove(entry);
        }

        internal List<NACHARecord> GetBatchRecords()
        {
            var records = new List<NACHARecord>();

            records.Add(_companyBatchHeader);

            var hasCredits = false;
            var hasDebits = false;

            // Reset composite fields
            _companyBatchControl.EntryHash = 0;
            _companyBatchControl.TotalCreditAmount = 0;
            _companyBatchControl.TotalDebitAmount = 0;

            foreach (var entry in _entries)
            {
                // Update the entry hash
                _companyBatchControl.EntryHash += entry.ReceivingDFIID;

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
            { ServiceClassCode = NACHAServiceClassCodeType.DebitsAndCredits; }
            else if (hasCredits)
            { ServiceClassCode = NACHAServiceClassCodeType.CreditsOnly; }
            else if (hasDebits)
            { ServiceClassCode = NACHAServiceClassCodeType.DebitsOnly; }

            // Fix the entry hash if it's too long. We want the right-most 10 digits, so can just take the entry hash mod 10000000000 (1 followed by 10 zeros)
            if (_companyBatchControl.EntryHash > 9999999999)
            {
                _companyBatchControl.EntryHash = _companyBatchControl.EntryHash % 10000000000;
            }

            _companyBatchControl.EntryAddendaCount = _entries.Count;

            records.Add(_companyBatchControl);

            return records;
        }

        internal static string FormatCompanyID(string input, int fieldLength)
        {
            return (input ?? "123456789").PadLeft(fieldLength, '1');
        }
    }
}
