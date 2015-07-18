using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    public class NACHAEntryDetailRecord : NACHARecord
    {
        public override NACHARecordType RecordTypeCode
        {
            get { return (Fields[0] as EnumeratedFixedWidthField<NACHARecordType>).Value; }
        }

        /// <summary>
        /// Transaction code for this entry object.
        /// </summary>
        public NACHATransactionCodeType TransactionCode
        {
            get
            {
                return (Fields[1] as EnumeratedFixedWidthField<NACHATransactionCodeType>).Value;
            }
            set
            {
                (Fields[1] as EnumeratedFixedWidthField<NACHATransactionCodeType>).Value = value;
            }
        }

        /// <summary>
        /// Indicates whether this transaction is a credit transaction by examining the Transaction Code
        /// </summary>
        public bool IsCredit
        {
            get
            {
                return TransactionCode == NACHATransactionCodeType.CreditChecking ||
                       TransactionCode == NACHATransactionCodeType.CreditSavings ||
                       TransactionCode == NACHATransactionCodeType.CreditCheckingPrenote ||
                       TransactionCode == NACHATransactionCodeType.CreditSavingsPrenote;
            }
        }

        /// <summary>
        /// Indicates whether this transaction is a debit transaction by examining the Transaction Code
        /// </summary>
        public bool IsDebit
        {
            get
            {
                return TransactionCode == NACHATransactionCodeType.DebitChecking ||
                       TransactionCode == NACHATransactionCodeType.DebitSavings ||
                       TransactionCode == NACHATransactionCodeType.DebitCheckingPrenote ||
                       TransactionCode == NACHATransactionCodeType.DebitSavingsPrenote;
            }
        }

        /// <summary>
        /// First 8 digits of the receiver's routing number. Can be set automatically by setting the ReceiverRoutingNumber property.
        /// </summary>
        public long ReceivingDFIID
        {
            get
            {
                return (Fields[2] as NumericNACHADataField).Value;
            }
            set
            {
                if (value < 0 || value > 99999999)
                {
                    throw new ArgumentException("ReceivingDFIID must be a non-negative number with fewer than 9 digits.");
                }

                (Fields[2] as NumericNACHADataField).Value = value;
            }
        }

        /// <summary>
        /// Check digit of the receiver's routing number, which is the last digit. Can be set automatically by setting the ReceiverRoutingNumber property.
        /// </summary>
        public long CheckDigit
        {
            get
            {
                return (Fields[3] as NumericNACHADataField).Value;
            }
            set
            {
                if (value < 0 || value > 9)
                {
                    throw new ArgumentException("CheckDigit must be a single digit.");
                }

                (Fields[3] as NumericNACHADataField).Value = value;
            }
        }

        /// <summary>
        /// Routing number of the bank at which the receiver's bank account is maintained.
        /// </summary>
        public string ReceiverRoutingNumber
        {
            get
            {
                return ReceivingDFIID.ToString(new string('0', 8)).Substring(0, 8) + CheckDigit.ToString("0");
            }
            set
            {
                // null check, pad on the left with zeros to 9 places, truncate to 9 places
                string input = (value ?? "").Trim().PadLeft(9, '0').Substring(0, 9);

                // ensure 9 consecutive digits
                if (Regex.IsMatch(input, @"^\d{9}$"))
                {
                    ReceivingDFIID = Convert.ToInt64(input.Substring(0, 8));
                    CheckDigit = Convert.ToInt64(input.Substring(8, 1));
                }
                else
                {
                    throw new ArgumentException("The value '" + value + "' is not a valid routing number.");
                }
            }
        }

        /// <summary>
        /// This is the receiver's bank account number. If the account number exceeds 17 positions, only use the left most 17 characters. Any spaces within the account number should be omitted when preparing the entry.
        /// </summary>
        public string DFIAccountNumber
        {
            get
            {
                return (Fields[4] as AlphamericNACHADataField).Value;
            }
            set
            {
                (Fields[4] as AlphamericNACHADataField).Value = value;
            }
        }

        /// <summary>
        /// The amount of the transaction. For prenotifications, the amount must be zero.
        /// </summary>
        public decimal Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                (Fields[5] as NumericNACHADataField).Value = (long)Math.Round(value * 100, 0);
            }
        } private decimal _amount;

        /// <summary>
        /// This is an identifying number by which the receiver is known to the originator. It is included for further identification and descriptive purposes.
        /// </summary>
        public string IndividualIdentificationNumber
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
        /// This is the name identifying the receiver of the transaction.
        /// </summary>
        public string ReceiverName
        {
            get
            {
                return (Fields[7] as AlphamericNACHADataField).Value;
            }
            set
            {
                (Fields[7] as AlphamericNACHADataField).Value = value;
            }
        }

        /// <summary>
        /// This is a means for the originator to identify individual entries. Must be unique relative to the file as a whole, but can repeat across several files.
        /// The trace numbers do not need to be sequential, but they must appear in ascending order.
        /// </summary>
        public long TraceNumber
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

        public NACHAEntryDetailRecord(NACHATransactionCodeType transactionCode)
        {
            // Define fields along with their type and length
            Fields = new List<IFixedWidthField>() {
                new EnumeratedFixedWidthField<NACHARecordType>(NACHARecordType.EntryDetail, 1), // Record Type Code
                new EnumeratedFixedWidthField<NACHATransactionCodeType>(transactionCode, 2), // Transaction Code -  Per NACHA docs, valid values are defined in the enumeration NACHATransactionCodeType
                new NumericNACHADataField(11111111, 8), // Receiving DFI ID - Per NACHA docs, this is the first 8 digits of the receiver's routing number. 11111111 is a testing routing number to be used if a routing number is not supplied.
                new NumericNACHADataField(8, 1), // Check Digit for the receiver's routing number - Per NACHA docs, this is the last digit of the routing number. 8 is the check digit of the testing routing number 111111118, which is only used if a routing number is not supplied.
                new AlphamericNACHADataField("", 17, FormatDFIAccountNumber), // DFI Account Number - Per NACHA docs, this is the receiver's account number. Must not contain spaces
                new NumericNACHADataField(0, 10), // Amount
                new AlphamericNACHADataField("", 15), // Individual Identification Number - Per NACHA docs, this is a value by which the originator knows the receiver.
                new AlphamericNACHADataField("", 22), // Individual Name or Receiving Company Name - Per NACHA docs, this is the name of the receiver
                new AlphamericNACHADataField("", 2), // Discretionary data - Per NACHA docs, this field is optional. Leaving blank.
                new NumericNACHADataField(0, 1), // Addenda Record Indicator - Per NACHA docs, this field should be 1 if there is an addenda attached to this entry and 0 otherwise.
                new NumericNACHADataField(0, 15) // Trace Number - Per NACHA docs, this field uniquely identifies the transaction relative to the rest of the file.
            };
        }

        private static string FormatDFIAccountNumber(string input, int maxLength)
        {
            return (input ?? "").Replace(" ", "").Trim();
        }
    }
}
