using System;
using System.Collections.Generic;

namespace Hyper.BatchProcessing
{
    public class BatchResult
    {
        #region Properties

        private List<string> _warnings = new List<string>();
        public List<string> Warnings
        {
            get { return _warnings; }
            internal set
            {
                _warnings = (value ?? new List<string>());
            }
        }

        private List<Exception> _errors = new List<Exception>();
        public List<Exception> Errors
        {
            get { return _errors; }
            internal set
            {
                _errors = (value ?? new List<Exception>());
            }
        }

        public bool HadWarnings { get { return (Warnings.Count > 0); } }
        public bool HadErrors { get { return (Errors.Count > 0); } }
        public long BatchItemNumber { get; set; }

        #endregion Properties
    }
}