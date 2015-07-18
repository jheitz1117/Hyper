using System;
using System.Collections.Generic;

namespace Hyper.UI.Validation
{
    public class HyperValidator
    {
        private List<ValidationStepBase> _validationSteps = new List<ValidationStepBase>();
        public List<ValidationStepBase> ValidationSteps
        {
            get
            {
                return _validationSteps;
            }
            set
            {
                _validationSteps = value ?? new List<ValidationStepBase>();
            }
        }

        /// <summary>
        /// Validates all validation steps in order. This method is guaranteed not to throw
        /// an exception. Instead, all exceptions are compiled and returned in the result
        /// object.
        /// </summary>
        /// <returns></returns>
        public HyperValidationResult Validate()
        {
            HyperValidationResult result = new HyperValidationResult();

            result.Success = true;

            foreach (ValidationStepBase step in ValidationSteps)
            {
                try
                {
                    if (step != null)
                    {
                        if (!step.IsValid())
                        {
                            result.ErrorMessages.Add(step.GetErrorMessage());
                            result.Success = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.ValidationExceptions.Add(ex);
                    result.ErrorMessages.Add(step.GetErrorMessage());
                    result.Success = false;
                }
            }

            return result;
        }
    }
}
