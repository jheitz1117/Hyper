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

        private ParameterizedColumnTransformDelegate<T> Transform
        {
            get;
            set;
        }

        #endregion Properties

        #region Public Methods

        public ParameterizedDataColumnTransform(ParameterizedColumnTransformDelegate<T> transform)
        {
            this.Transform = transform;
        }

        public ParameterizedDataColumnTransform(ParameterizedColumnTransformDelegate<T> transform, T parameter)
            : this(transform)
        {
            this.Parameter = parameter;
        }

        public ParameterizedDataColumnTransform(ParameterizedColumnTransformDelegate<T> transform, int executionOrder)
            : this(transform)
        {
            this.ExecutionOrder = executionOrder;
        }

        public ParameterizedDataColumnTransform(ParameterizedColumnTransformDelegate<T> transform, T parameter, int executionOrder)
            : this(transform, executionOrder)
        {
            this.Parameter = parameter;
        }

        public override string TransformValue(string inputValue)
        {
            if (Transform == null)
            {
                return inputValue;
            }

            return Transform(inputValue, Parameter);
        }

        #endregion Public Methods
    }
}