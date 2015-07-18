using System;

namespace Hyper.FileProcessing.Parsing.Transforms
{
    public static class StandardTransforms {
        #region Public Methods

        public static string Truncate(string input, int maxLength) {
            return ParsingHelper.Truncate(input, maxLength);
        }

        public static string StripNonNumeric(string input) {
            return ParsingHelper.StripNonNumeric(input);
        }

        public static string StripNonDecimal(string input) {
            return ParsingHelper.StripNonDecimal(input);
        }

        public static string ToBooleanString(string input, ToBooleanStringParameter parameter) {
            return ToBooleanString(input, parameter.TrueString, parameter.FalseString, parameter.CaseSensitive);
        }

        public static string Replace(string input, ReplaceParameter parameter) {
            return Replace(input, parameter.TargetString, parameter.ReplacementString, parameter.CaseSensitive);
        }

        /// <summary>
        /// Performs a 1-to-1 transformation from the strings (trueString,falseString) to ("True","False") respectively. Any input values other than
        /// the specified trueString or falseString values are transformed to the empty string
        /// </summary>
        /// <param name="input">The value to transform</param>
        /// <param name="trueString">The string to be transformed into "True"</param>
        /// <param name="falseString">The string to be transformed into "False"</param>
        /// <param name="caseSensitive">Indicates whether the strings are case-sensitive</param>
        /// <returns></returns>
        public static string ToBooleanString(string input, string trueString, string falseString, bool caseSensitive = false) {
            string returnValue = null;

            input = (input ?? "").Trim();
            trueString = (trueString ?? "").Trim();
            falseString = (falseString ?? "").Trim();

            StringComparison comparisonType = (caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
            if (input.Equals(trueString, comparisonType)) {
                returnValue = Boolean.TrueString;
            } else if (input.Equals(falseString, comparisonType)) {
                returnValue = Boolean.FalseString;
            } else {
                returnValue = "";
            }

            return returnValue;
        }

        public static string PadLeft(string inputValue, PadStringParameter parameter) {
            return (inputValue ?? "").PadLeft(parameter.MaxLength, parameter.PadChar);
        }

        public static string PadRight(string inputValue, PadStringParameter parameter) {
            return (inputValue ?? "").PadRight(parameter.MaxLength, parameter.PadChar);
        }

        public static string DateTransformExact(string inputValue, string dateFormatString) {
            inputValue = (inputValue ?? "");

            DateTime outputDate = new DateTime();
            if (DateTime.TryParseExact(inputValue, dateFormatString, null, System.Globalization.DateTimeStyles.None, out outputDate)) {
                return outputDate.ToString();
            } else {
                return "";
            }
        }

        /// <summary>
        /// Checks to see if the specified input value matches the target string. If it does, the specified replacement string is returned.
        /// </summary>
        /// <param name="input">Input value to consider</param>
        /// <param name="targetString">Target string to replace</param>
        /// <param name="replacementString">Replacement string</param>
        /// <param name="caseSensitive">Indicates whether or not to match case</param>
        /// <returns></returns>
        public static string Replace(string input, string targetString, string replacementString, bool caseSensitive = false) {
            targetString = (targetString ?? "").Trim();
            replacementString = (replacementString ?? "").Trim();

            StringComparison comparisonType = (caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
            if ((input ?? "").Trim().Equals(targetString, comparisonType)) {
                return replacementString;
            } else {
                return input;
            }
        }

        #endregion Public Methods
    }
}