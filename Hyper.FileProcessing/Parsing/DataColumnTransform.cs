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

        private ColumnTransformDelegate Transform { get; }

        #endregion Properties

        #region Public Methods

        protected DataColumnTransform()
        {
            Transform = IdentityTransform;
        }

        public DataColumnTransform(ColumnTransformDelegate transform)
        {
            Transform = transform;
        }

        public DataColumnTransform(ColumnTransformDelegate transform, int executionOrder)
            : this(transform)
        {
            ExecutionOrder = executionOrder;
        }

        public virtual string TransformValue(string inputValue)
        {
            return Transform == null ? inputValue : Transform(inputValue);
        }

        #endregion Public Methods

        #region Protected Methods

        protected static string IdentityTransform(string inputValue)
        {
            return inputValue;
        }

        #endregion Protected Methods
    }
}