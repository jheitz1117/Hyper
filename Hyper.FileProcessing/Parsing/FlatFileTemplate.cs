using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

namespace Hyper.FileProcessing.Parsing
{
    public class FlatFileTemplate {
        #region Properties
        
        public string[] Delimiters {
            get;
            set;
        }

        public int[] FieldWidths {
            get;
            set;
        }

        public bool HasFieldsEnclosedInQuotes {
            get;
            set;
        }

        public FieldType TextFieldType {
            get;
            set;
        }

        public bool TrimWhiteSpace {
            get;
            set;
        }

        private List<string> _columns = new List<string>();
        public List<string> Columns {
            get {
                return _columns;
            }
            set {
                value = (value ?? new List<string>());
                _columns = value;
            }
        }

        public int HeaderLinesToSkip {
            get;
            set;
        }

        #endregion Properties

        #region Public Methods

        public FlatFileTemplate() {
            Delimiters = new string[] { "," };
            HasFieldsEnclosedInQuotes = true;
            TextFieldType = FieldType.Delimited;
            TrimWhiteSpace = true;
        } // end FeedFileTemplate()

        public FlatFileTemplate(string[] delimiters)
            : this() {
            Delimiters = delimiters;
        } // end FeedFileTemplate()

        public FlatFileTemplate(string[] delimiters, bool hasFieldsEnclosedInQuotes)
            : this(delimiters) {
            HasFieldsEnclosedInQuotes = hasFieldsEnclosedInQuotes;
        } // end FeedFileTemplate()

        public FlatFileTemplate(string delimiter)
            : this(new string[] { delimiter }) {
        } // end FeedFileTemplate()

        public FlatFileTemplate(string delimiter, bool hasFieldsEnclosedInQuotes)
            : this(new string[] { delimiter }, hasFieldsEnclosedInQuotes) {
        } // end FeedFileTemplate()

        #endregion Public Methods
    }
}