using System.Collections.Generic;
using Hyper.BatchProcessing;

namespace Hyper.FileProcessing.Parsing
{
    public class ObjectMappingResult<T> where T : new()
    {
        #region Properties

        private List<T> _transformedItems = new List<T>();
        public List<T> TransformedItems
        {
            get
            {
                return _transformedItems;
            }
            internal set
            {
                _transformedItems = (value ?? new List<T>());
            }
        }

        private List<T> _partialItems = new List<T>();
        public List<T> PartialItems
        {
            get
            {
                return _partialItems;
            }
            internal set
            {
                _partialItems = (value ?? new List<T>());
            }
        }

        private List<BatchResult> _batchResults = new List<BatchResult>();
        public List<BatchResult> BatchResults
        {
            get
            {
                return _batchResults;
            }
            internal set
            {
                _batchResults = (value ?? new List<BatchResult>());
            }
        }

        public bool HadWarnings
        {
            get;
            set;
        }

        public bool HadErrors
        {
            get;
            set;
        }

        #endregion Properties
    }
}