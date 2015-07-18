namespace Hyper.FileProcessing.Parsing
{
    public delegate string ColumnTransformDelegate(string inputValue);

    public class DataColumnTransform
    {
        #region Properties

        public int ExecutionOrder
        {
            get;
            set;
        }

        private ColumnTransformDelegate Transform
        {
            get;
            set;
        }

        #endregion Properties

        #region Public Methods

        protected DataColumnTransform()
        {
            this.Transform = IdentityTransform;
        }

        public DataColumnTransform(ColumnTransformDelegate transform)
        {
            this.Transform = transform;
        }

        public DataColumnTransform(ColumnTransformDelegate transform, int executionOrder)
            : this(transform)
        {
            this.ExecutionOrder = executionOrder;
        }

        public virtual string TransformValue(string inputValue)
        {
            if (Transform == null)
            {
                return inputValue;
            }

            return Transform(inputValue);
        }

        #endregion Public Methods

        #region Protected Methods

        protected static string IdentityTransform(string inputValue)
        {
            return inputValue;
        } // end IdentityTransform()

        #endregion Protected Methods
    }
}