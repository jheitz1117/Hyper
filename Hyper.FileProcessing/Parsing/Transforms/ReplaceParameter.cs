namespace Hyper.FileProcessing.Parsing.Transforms
{
    public class ReplaceParameter {
        #region Properties

        public string TargetString {
            get;
            set;
        }

        public string ReplacementString {
            get;
            set;
        }

        public bool CaseSensitive {
            get;
            set;
        }

        #endregion Properties

        #region Public Methods

        public ReplaceParameter() {
        }

        public ReplaceParameter(string targetString, string replacementString)
            : this() {
                TargetString = targetString;
                ReplacementString = replacementString;
        }

        public ReplaceParameter(string targetString, string replacementString, bool caseSensitive)
            : this(targetString, replacementString) {
            CaseSensitive = caseSensitive;
        }

        #endregion Public Methods
    }
}
