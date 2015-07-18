using System;
using System.Collections.Generic;
using Hyper.FileProcessing.FixedWidthFiles;

namespace Hyper.NACHA
{
    internal class NACHAFileHeaderRecord : NACHARecord
    {
        public override NACHARecordType RecordTypeCode
        {
            get { return (Fields[0] as EnumeratedFixedWidthField<NACHARecordType>).Value; }
        }

        /// <summary>
        /// Unused at this time. Must be "01" and is hard-coded as such.
        /// </summary>
        public long PriorityCode
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

        /// <summary>
        /// The routing number of the destination financial institution
        /// </summary>
        public string ImmediateDestination
        {
            get
            {
                return (Fields[2] as AlphamericNACHADataField).Value;
            }
            set
            {
                (Fields[2] as AlphamericNACHADataField).Value = value;
            }
        }

        /// <summary>
        /// The Tax ID of the company originating (creating) the file
        /// </summary>
        public string ImmediateOrigin
        {
            get
            {
                return (Fields[3] as AlphamericNACHADataField).Value;
            }
            set
            {
                (Fields[3] as AlphamericNACHADataField).Value = value;
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
                (Fields[4] as AlphamericNACHADataField).Value = value.ToString("yyMMdd");
                (Fields[5] as AlphamericNACHADataField).Value = value.ToString("HHmm");
            }
        }

        /// <summary>
        /// Provides a way for an originator to distinguish between multiple files created on the same date.
        /// </summary>
        public string FileIDModifier
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

        public long RecordSize
        {
            get
            {
                return (Fields[7] as NumericNACHADataField).Value;
            }
        }

        public long BlockingFactor
        {
            get
            {
                return (Fields[8] as NumericNACHADataField).Value;
            }
        }

        public long FormatCode
        {
            get
            {
                return (Fields[9] as NumericNACHADataField).Value;
            }
        }

        /// <summary>
        /// The name of the destination financial institution
        /// </summary>
        public string ImmediateDestinationName
        {
            get
            {
                return (Fields[10] as AlphabeticNACHADataField).Value;
            }
            set
            {
                (Fields[10] as AlphabeticNACHADataField).Value = value;
            }
        }

        /// <summary>
        /// The name of the originating company
        /// </summary>
        public string ImmediateOriginName
        {
            get
            {
                return (Fields[11] as AlphabeticNACHADataField).Value;
            }
            set
            {
                (Fields[11] as AlphabeticNACHADataField).Value = value;
            }
        }

        public NACHAFileHeaderRecord(int blockingFactor, int formatCode)
        {
            // Define fields along with their type and length
            Fields = new List<IFixedWidthField>() {
                new EnumeratedFixedWidthField<NACHARecordType>(NACHARecordType.FileHeader, 1), // Record Type Code
                new NumericNACHADataField(1, 2), // Priority Code -  Per NACHA docs, this field is not used and must have a value of 01
                new AlphamericNACHADataField("", 10, FormatImmediateDestination), // Immediate Destination - 111111118 is a testing routing number to be used if a routing number is not supplied
                new AlphamericNACHADataField("", 10, FormatImmediateOrigin), // Immediate Origin
                new AlphamericNACHADataField(_fileCreateDate.ToString("yyMMdd"), 6), // File Create or Transmission Date
                new AlphamericNACHADataField(_fileCreateDate.ToString("HHmm"), 4), // File Create or Transmission Time
                new AlphamericNACHADataField("A", 1), // File ID Modifier - provides a way for an originator to distinguish between multiple files created on the same date.
                new NumericNACHADataField(Length, 3), // Record Size
                new NumericNACHADataField(blockingFactor, 2), // Blocking Factor - Defines how many records make up a single block. A NACHA file must consist only of whole blocks.
                new NumericNACHADataField(formatCode, 1), // Format Code
                new AlphabeticNACHADataField("", 23), // Immediate Destination Name - The name of the destination financial institution
                new AlphabeticNACHADataField("", 23), // Immediate Origin Name - The name of the destination financial institution
                new AlphabeticNACHADataField("", 8) // Reference Code - Per NACHA docs, filled with blanks
            };
        }

        public NACHAFileHeaderRecord(int blockingFactor, int formatCode, DateTime fileCreateDate)
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
