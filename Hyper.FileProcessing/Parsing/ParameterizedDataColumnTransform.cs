namespace Hyper.FileProcessing.Parsing
{
    public delegate string ParameterizedColumnTransformDelegate<in T>(string inputValue, T parameter);

    public class ParameterizedDataColumnTransform<T> : DataColumnTransform
    {
        #region Properties

        public T Parameter
        {
            get;
            set;
        }

        private ParameterizedColumnTransformDelegate<T> Transform { get; }

        #endregion Properties

        #region Public Methods

        public ParameterizedDataColumnTransform(ParameterizedColumnTransformDelegate<T> transform)
        {
            Transform = transform;
        }

        public ParameterizedDataColumnTransform(ParameterizedColumnTransformDelegate<T> transform, T parameter)
            : this(transform)
        {
            Parameter = parameter;
        }

        public ParameterizedDataColumnTransform(ParameterizedColumnTransformDelegate<T> transform, int executionOrder)
            : this(transform)
        {
            ExecutionOrder = executionOrder;
        }

        public ParameterizedDataColumnTransform(ParameterizedColumnTransformDelegate<T> transform, T parameter, int executionOrder)
            : this(transform, executionOrder)
        {
            Parameter = parameter;
        }

        public override string TransformValue(string inputValue)
        {
            return Transform == null ? inputValue : Transform(inputValue, Parameter);
        }

        #endregion Public Methods
    }
}