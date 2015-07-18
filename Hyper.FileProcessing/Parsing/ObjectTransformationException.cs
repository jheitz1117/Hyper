using System;
using System.Runtime.Serialization;

namespace Hyper.FileProcessing.Parsing
{
    [Serializable]
    public class ObjectTransformationException : ApplicationException {
        #region Public Methods

        public ObjectTransformationException()
            : base() {
        }

        public ObjectTransformationException(string message)
            : base(message) {
        }

        public ObjectTransformationException(string message, Exception innerException)
            : base(message, innerException) {
        }

        public ObjectTransformationException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        #endregion Public Methods
    }
}
