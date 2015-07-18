using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hyper.UI
{
    public class HyperRecipientInfo
    {
        public const string DefaultRegexForAcceptedSeparators = "[\\s;]";
        public const char DefaultPreferredSeparator = ',';

        #region Properties

        private string _regexForAcceptedSeparators = DefaultRegexForAcceptedSeparators;
        public string RegexForAcceptedSeparators
        {
            get
            {
                return _regexForAcceptedSeparators;
            }
            set
            {
                _regexForAcceptedSeparators = value ?? "";
            }
        }

        private char _preferredSeparator = DefaultPreferredSeparator;
        public char PreferredSeparator
        {
            get
            {
                return _preferredSeparator;
            }
            set
            {
                _preferredSeparator = value;
            }
        }

        #endregion Properties

        #region Public Methods

        public HyperRecipientInfo() { }
        public HyperRecipientInfo(string regexForAcceptedSeparators, char preferredSeparator)
            : this()
        {
            this.RegexForAcceptedSeparators = regexForAcceptedSeparators;
            this.PreferredSeparator = preferredSeparator;
        }

        public static List<string> GetCondensedRecipients(string rawRecipientList)
        {
            return new List<string>(Regex.Replace(rawRecipientList ?? "", DefaultRegexForAcceptedSeparators, DefaultPreferredSeparator.ToString()).Split(new char[] { DefaultPreferredSeparator }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static string GetCondensedRecipients(string rawRecipientList, string separator)
        {
            return string.Join(separator, GetCondensedRecipients(rawRecipientList).ToArray());
        }

        #endregion Public Methods
    }
}