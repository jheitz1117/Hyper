namespace Hyper.UI.Validation
{
    public abstract class ValidationStepBase
    {
        public string InvalidErrorMessage
        {
            get;
            protected set;
        }

        public virtual bool IsValid()
        {
            return false;
        }

        /// <summary>
        /// When overridden in a derived class, returns the appropriate error message to display based on the validity of the object.
        /// When this method is not overridden, it simply wraps the getter for the InvalidErrorMessage property.
        /// </summary>
        /// <returns></returns>
        public virtual string GetErrorMessage()
        {
            return InvalidErrorMessage;
        }
    }
}
