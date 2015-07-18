using System;

namespace Hyper.UI.Validation
{
    /// <summary>
    /// Defines a validation step that takes an arbitrary value and
    /// evaluates it in order to determine whether the value is valid.
    /// All input field validation (such as emails, phone numbers, etc.)
    /// should be done using this class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueValidationStep<T> : CustomValidationStep
    {

        #region Properties

        /// <summary>
        /// The value to be validated
        /// </summary>
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        /// Validation function used to determine whether the value is valid
        /// </summary>
        public Func<T, bool> ValueValidationFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether the value is required
        /// </summary>
        public bool IsRequired
        {
            get;
            set;
        }

        /// <summary>
        /// If the value is blank, this error message is displayed. This error message takes precedence over the InvalidErrorMessage parameter.
        /// </summary>
        public string RequiredErrorMessage
        {
            get;
            set;
        }

        private bool ValueIsRequiredAndBlank
        {
            get
            {
                return (IsRequired && ValueIsBlank());
            }
        }

        #endregion Properties

        #region Public Methods

        public ValueValidationStep(T value, Func<T, bool> validationFunction)
        {
            this.Value = value;
            this.ValueValidationFunction = validationFunction;
        }

        public ValueValidationStep(T value, Func<T, bool> validationFunction, bool isRequired)
            : this(value, validationFunction)
        {
            this.IsRequired = isRequired;
        }

        /// <summary>
        /// If the value is required and blank, returns the value of the RequiredErrorMessage property. Otherwise, returns the value of the InvalidErrorMessage property
        /// </summary>
        /// <returns></returns>
        public override string GetErrorMessage()
        {
            if (ValueIsRequiredAndBlank)
            {
                return RequiredErrorMessage;
            }
            else
            {
                return InvalidErrorMessage;
            }
        }

        public override bool IsValid()
        {
            if (ValueIsBlank())
            {
                return !IsRequired;
            }
            else if (ValueValidationFunction == null)
            {
                // We have a non-blank value and no validation function. It is safer to assume the value is valid until proven invalid by the specified validation function.
                return true;
            }
            else
            {
                // We have a non-blank value and a non-null validation function, so validate and return the result
                return ValueValidationFunction(Value);
            }
        }

        #endregion Public Methods

        #region Private Methods

        protected virtual bool ValueIsBlank()
        {
            bool result = false;

            if (Value == null)
            {
                result = true;
            }
            else if (Value is string)
            {
                result = string.IsNullOrWhiteSpace(Value as string);
            }

            return result;
        }

        #endregion Private Methods
    }
}
