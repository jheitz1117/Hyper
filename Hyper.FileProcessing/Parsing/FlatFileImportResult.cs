using System.Collections.Generic;
using Hyper.BatchProcessing;

namespace Hyper.FileProcessing.Parsing
{
    public class FlatFileImportResult : BatchResult
    {
        #region Properties

        private List<Dictionary<string, string>> _importedData = new List<Dictionary<string, string>>();
        public List<Dictionary<string, string>> ImportedData
        {
            get
            {
                return _importedData;
            }
            set
            {
                _importedData = value ?? new List<Dictionary<string, string>>();
            }
        }

        private List<string> _headerLines = new List<string>();
        public List<string> HeaderLines
        {
            get
            {
                return _headerLines;
            }
            set
            {
                _headerLines = value ?? new List<string>();
            }
        }

        public int SkippedRecords
        {
            get;
            set;
        }

        public int TotalRecords
        {
            get;
            set;
        }

        #endregion Properties
    }
}