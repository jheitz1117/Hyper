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

        private bool ValueIsRequiredAndBlank => IsRequired && ValueIsBlank();

        #endregion Properties

        #region Public Methods

        public ValueValidationStep(T value, Func<T, bool> validationFunction)
        {
            Value = value;
            ValueValidationFunction = validationFunction;
        }

        public ValueValidationStep(T value, Func<T, bool> validationFunction, bool isRequired)
            : this(value, validationFunction)
        {
            IsRequired = isRequired;
        }

        /// <summary>
        /// If the value is required and blank, returns the value of the RequiredErrorMessage property. Otherwise, returns the value of the InvalidErrorMessage property
        /// </summary>
        /// <returns></returns>
        public override string GetErrorMessage()
        {
            return ValueIsRequiredAndBlank ? RequiredErrorMessage : InvalidErrorMessage;
        }

        public override bool IsValid()
        {
            if (ValueIsBlank())
                return !IsRequired;

            // We have a non-blank value and a non-null validation function, so validate and return the result
            return ValueValidationFunction == null || ValueValidationFunction(Value);
            
        }

        #endregion Public Methods

        #region Private Methods

        protected virtual bool ValueIsBlank()
        {
            var result = false;

            if (Value == null)
                result = true;
            else if (Value is string)
                result = string.IsNullOrWhiteSpace(Value as string);

            return result;
        }

        #endregion Private Methods
    }
}
