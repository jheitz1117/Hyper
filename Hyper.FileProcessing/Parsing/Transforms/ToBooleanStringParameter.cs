namespace Hyper.FileProcessing.Parsing.Transforms
{
    public class ToBooleanStringParameter {
        #region Properties

        public string TrueString {
            get;
            set;
        }

        public string FalseString {
            get;
            set;
        }

        public bool CaseSensitive {
            get;
            set;
        }

        #endregion Properties

        #region Public Methods

        public ToBooleanStringParameter() {
        }

        public ToBooleanStringParameter(string trueString, string falseString)
            : this() {
            TrueString = trueString;
            FalseString = falseString;
        }

        public ToBooleanStringParameter(string trueString, string falseString, bool caseSensitive)
            : this(trueString, falseString) {
            CaseSensitive = caseSensitive;
        }

        #endregion Public Methods
    }
}
