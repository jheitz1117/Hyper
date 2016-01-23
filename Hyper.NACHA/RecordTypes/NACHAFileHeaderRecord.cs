using System;
using System.Collections.Generic;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NachaFileHeaderRecord : NachaRecord
    {
        public override NachaRecordType RecordTypeCode => ((EnumeratedFixedWidthField<NachaRecordType>) Fields[0]).Value;

        /// <summary>
        /// Unused at this time. Must be "01" and is hard-coded as such.
        /// </summary>
        public long PriorityCode
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

        /// <summary>
        /// The routing number of the destination financial institution
        /// </summary>
        public string ImmediateDestination
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
        /// The Tax ID of the company originating (creating) the file
        /// </summary>
        public string ImmediateOrigin
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

        private DateTime _fileCreateDate = DateTime.Now;
        public DateTime FileCreateDate
        {
            get
            {
                return _fileCreateDate;
            }
            set
            {
                _fileCreateDate = value;
                ((AlphamericNachaDataField) Fields[4]).Value = value.ToString("yyMMdd");
                ((AlphamericNachaDataField) Fields[5]).Value = value.ToString("HHmm");
            }
        }

        /// <summary>
        /// Provides a way for an originator to distinguish between multiple files created on the same date.
        /// </summary>
        public string FileIdModifier
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

        public long RecordSize => ((NumericNachaDataField) Fields[7]).Value;

        public long BlockingFactor => ((NumericNachaDataField) Fields[8]).Value;

        public long FormatCode => ((NumericNachaDataField) Fields[9]).Value;

        /// <summary>
        /// The name of the destination financial institution
        /// </summary>
        public string ImmediateDestinationName
        {
            get
            {
                return (Fields[10] as AlphabeticNachaDataField)?.Value;
            }
            set
            {
                ((AlphabeticNachaDataField) Fields[10]).Value = value;
            }
        }

        /// <summary>
        /// The name of the originating company
        /// </summary>
        public string ImmediateOriginName
        {
            get
            {
                return (Fields[11] as AlphabeticNachaDataField)?.Value;
            }
            set
            {
                ((AlphabeticNachaDataField) Fields[11]).Value = value;
            }
        }

        public NachaFileHeaderRecord(int blockingFactor, int formatCode)
        {
            // Define fields along with their type and length
            Fields = new List<IFixedWidthField>() {
                new EnumeratedFixedWidthField<NachaRecordType>(NachaRecordType.FileHeader, 1), // Record Type Code
                new NumericNachaDataField(1, 2), // Priority Code -  Per NACHA docs, this field is not used and must have a value of 01
                new AlphamericNachaDataField("", 10, FormatImmediateDestination), // Immediate Destination - 111111118 is a testing routing number to be used if a routing number is not supplied
                new AlphamericNachaDataField("", 10, FormatImmediateOrigin), // Immediate Origin
                new AlphamericNachaDataField(_fileCreateDate.ToString("yyMMdd"), 6), // File Create or Transmission Date
                new AlphamericNachaDataField(_fileCreateDate.ToString("HHmm"), 4), // File Create or Transmission Time
                new AlphamericNachaDataField("A", 1), // File ID Modifier - provides a way for an originator to distinguish between multiple files created on the same date.
                new NumericNachaDataField(Length, 3), // Record Size
                new NumericNachaDataField(blockingFactor, 2), // Blocking Factor - Defines how many records make up a single block. A NACHA file must consist only of whole blocks.
                new NumericNachaDataField(formatCode, 1), // Format Code
                new AlphabeticNachaDataField("", 23), // Immediate Destination Name - The name of the destination financial institution
                new AlphabeticNachaDataField("", 23), // Immediate Origin Name - The name of the destination financial institution
                new AlphabeticNachaDataField("", 8) // Reference Code - Per NACHA docs, filled with blanks
            };
        }

        public NachaFileHeaderRecord(int blockingFactor, int formatCode, DateTime fileCreateDate)
            : this(blockingFactor, formatCode)
        {
            FileCreateDate = fileCreateDate;
        }

        private static string FormatImmediateDestination(string input, int fieldLength)
        {
            return (input ?? "111111118").PadLeft(fieldLength, ' ');
        }

        private static string FormatImmediateOrigin(string input, int fieldLength)
        {
            input = input ?? "123456789";
            if (input.Length > 0 && input.Length != 9 && input.Length != 10)
            { throw new ArgumentException("Immediate Origin must be 9 or 10 digits long."); }

            return input.PadLeft(fieldLength, '1');
        }
    }
}
