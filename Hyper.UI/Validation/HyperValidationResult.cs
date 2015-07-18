using System;
using System.Collections.Generic;
using System.Linq;

namespace Hyper.UI.Validation
{
    public class HyperValidationResult
    {
        public bool Success
        {
            get;
            set;
        }

        private List<Exception> _validationExceptions = new List<Exception>();
        public List<Exception> ValidationExceptions
        {
            get
            {
                return _validationExceptions;
            }
            set
            {
                _validationExceptions = value ?? new List<Exception>();
            }
        }

        private List<string> _errorMessages = new List<string>();
        public List<string> ErrorMessages
        {
            get
            {
                return _errorMessages;
            }
            set
            {
                _errorMessages = value ?? new List<string>();
            }
        }

        public string GetErrorMessageString(string separator, bool includeExceptionMessages = false)
        {
            separator = separator ?? "";
            string errorString = string.Join(separator, ErrorMessages.ToArray());
            if (includeExceptionMessages)
            {
                if (!string.IsNullOrWhiteSpace(errorString))
                {
                    errorString += separator;
                }
                errorString += string.Join(separator, ValidationExceptions.Select(x => x.Message).ToArray());
            }

            return errorString;
        }
    }
}
