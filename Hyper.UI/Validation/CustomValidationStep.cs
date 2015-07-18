using System;

namespace Hyper.UI.Validation
{
    /// <summary>
    /// Defines a validation step that takes an arbitrary validation function to
    /// evaluate. This function may evaluate some external condition that does
    /// not depend on the internal state of this validation step. This class is
    /// not designed to evaluate a particular value to determine its validity,
    /// but rather other even more abritrary conditions such as whether at least
    /// one of a set of values is selected, or a web service call returns a
    /// successful response
    /// </summary>
    public class CustomValidationStep : ValidationStepBase
    {
        private Func<bool> _validationFunction = null;

        protected CustomValidationStep() { } // required so that the generic class ValueValidationStep can inherit from this class
        public CustomValidationStep(Func<bool> validationFunction)
        {
            this._validationFunction = validationFunction;
        }

        public override bool IsValid()
        {
            if (_validationFunction != null)
            {
                return _validationFunction();
            }

            return base.IsValid();
        }
    }
}
