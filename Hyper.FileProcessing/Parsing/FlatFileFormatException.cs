﻿using System;
using System.Runtime.Serialization;

namespace Hyper.FileProcessing.Parsing
{
    [Serializable]
    public class FlatFileFormatException : Exception
    {
        #region Public Methods

        public FlatFileFormatException() { }

        public FlatFileFormatException(string message)
            : base(message) { }

        public FlatFileFormatException(string message, Exception innerException)
            : base(message, innerException) { }

        public FlatFileFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion Public Methods
    }
}