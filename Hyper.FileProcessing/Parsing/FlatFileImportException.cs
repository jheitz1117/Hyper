using System;
using System.Runtime.Serialization;

namespace Hyper.FileProcessing.Parsing
{
    [Serializable]
    public class FlatFileImportException : Exception
    {
        #region Public Methods

        public FlatFileImportException() { }

        public FlatFileImportException(string message)
            : base(message) { }

        public FlatFileImportException(string message, Exception innerException)
            : base(message, innerException) { }

        public FlatFileImportException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion Public Methods
    }
}