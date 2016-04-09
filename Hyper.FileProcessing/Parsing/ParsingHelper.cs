using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hyper.FileProcessing.Parsing
{
    public static class ParsingHelper
    {
        public static long GetNumericValue(string input, long defaultValue = 0)
        {
            long numericValue;
            return long.TryParse(StripNonNumeric(input), out numericValue) ? numericValue : defaultValue;
        }

        public static long? GetNumericValue(string input, long? defaultValue = null)
        {
            long numericValue;
            return long.TryParse(StripNonNumeric(input), out numericValue) ? numericValue : defaultValue;
        }

        public static int GetNumericValue(string input, int defaultValue = 0)
        {
            int numericValue;
            return int.TryParse(StripNonNumeric(input), out numericValue) ? numericValue : defaultValue;
        }

        public static int? GetNumericValue(string input, int? defaultValue = null)
        {
            int numericValue;
            return int.TryParse(StripNonNumeric(input), out numericValue) ? numericValue : defaultValue;
        }

        public static byte GetNumericValue(string input, byte defaultValue = 0)
        {
            byte numericValue;
            return byte.TryParse(StripNonNumeric(input), out numericValue) ? numericValue : defaultValue;
        }

        public static byte? GetNumericValue(string input, byte? defaultValue = null)
        {
            byte numericValue;
            return byte.TryParse(StripNonNumeric(input), out numericValue) ? numericValue : defaultValue;
        }

        public static decimal GetNumericValue(string input, decimal defaultValue = 0)
        {
            decimal numericValue;
            return decimal.TryParse(StripNonNumeric(input), out numericValue) ? numericValue : defaultValue;
        }

        public static decimal? GetNumericValue(string input, decimal? defaultValue = null)
        {
            decimal numericValue;
            return decimal.TryParse(StripNonDecimal(input), out numericValue) ? numericValue : defaultValue;
        }

        public static bool IsValidEmail(string input)
        {
            return IsValidEmail(input, false);
        }

        public static bool IsValidEmail(string input, bool allowNullOrWhiteSpace)
        {
            if (string.IsNullOrWhiteSpace(input) && allowNullOrWhiteSpace)
            { return true; }

            return Regex.IsMatch(input ?? "", @"^[a-zA-Z0-9.-_]+@[a-zA-Z0-9.-_]+\.[a-zA-Z0-9.-_]+$");
        }

        public static bool IsValidPhoneNumber(long? input)
        {
            var phoneNumberLength = input.ToString().Length;
            return phoneNumberLength == 7 ||
                   phoneNumberLength == 10 ||
                   phoneNumberLength == 11;
        }

        public static bool IsStringNumeric(string input)
        {
            return IsStringNumeric(input, false);
        }

        public static bool IsGreaterThanZero(double? input)
        {
            return (input ?? 0) > 0;
        }

        public static bool IsGreaterThanZero(long? input)
        {
            return (input ?? 0) > 0;
        }

        public static bool IsGreaterThanZero(int? input)
        {
            return (input ?? 0) > 0;
        }

        public static bool IsGreaterThanZero(short? input)
        {
            return (input ?? 0) > 0;
        }

        public static bool IsGreaterThanZero(byte? input)
        {
            return (input ?? 0) > 0;
        }

        public static bool IsStringNumeric(string input, bool allowDecimal)
        {
            var pattern = @"^\d+$";
            if (allowDecimal)
                pattern = @"^(\d+\.?\d*|\d*\.?\d+|)$";

            return Regex.IsMatch(input ?? "", pattern);
        }

        public static bool IsValidYear(string input)
        {
            int year;
            return int.TryParse(input, out year) && IsValidYear(year);
        }

        public static bool IsValidYear(int? input)
        {
            return input > 0 && input <= 9999;
        }

        public static bool IsValidZipCode(long? input)
        {
            return input > 0 && input <= 999999999;
        }

        public static string Truncate(string input, int maxLength)
        {
            return input?.Substring(0, Math.Min(input.Length, maxLength));
        }

        public static string StripNonNumeric(string input)
        {
            return Regex.Replace(input ?? "", "[^0-9]", "");
        }

        public static string StripNonDecimal(string input)
        {
            // Strip out everything that isn't a digit or a decimal
            var returnValue = Regex.Replace(input ?? "", "[^0-9.]", "");

            // Only keep the first decimal
            var firstDecimalIndex = returnValue.IndexOf('.');
            if (firstDecimalIndex >= 0)
                returnValue = returnValue.Replace(".", "").Insert(firstDecimalIndex, ".");

            return returnValue;
        }

        /// <summary>
        /// Returns a list of strings which are no longer than the specified max length, and each of which start and end with the specified prefix and suffix respectively.
        /// The first string in the list does not begin with the prefix, and the last string does not end with the suffix.
        /// </summary>
        /// <param name="input">Input string to break apart</param>
        /// <param name="maxLineLength">Max length for each substring, including the prefix and suffix</param>
        /// <param name="linePrefix">String to prefix each line</param>
        /// <param name="lineSuffix">String to suffix each line</param>
        /// <returns></returns>
        public static List<string> WrapString(string input, int maxLineLength, string linePrefix = "", string lineSuffix = "")
        {
            var lines = new List<string>();

            // Recalculate the max line length to account for the prefix and suffix
            if (!string.IsNullOrWhiteSpace(linePrefix))
                maxLineLength -= linePrefix.Length;
            if (!string.IsNullOrWhiteSpace(lineSuffix))
                maxLineLength -= lineSuffix.Length;

            if (string.IsNullOrWhiteSpace(input))
                return lines;

            if (maxLineLength <= 0 || input.Length <= maxLineLength)
            {
                lines.Add(input);
            }
            else
            {
                var words = new List<string>(Regex.Replace(input, @"\s+", " ").Split(' '));

                // Make sure none of the individual words are longer than our max line length
                bool foundLongWord;
                do
                {
                    foundLongWord = false;

                    for (var i = 0; i < words.Count; i++)
                    {
                        foundLongWord |= (words[i].Length > maxLineLength);

                        // If we found a long word, break it into smaller words
                        if (foundLongWord)
                        {
                            var subWords = new List<string>();
                            var numSubWords = (int)Math.Ceiling((double)words[i].Length / maxLineLength);
                            for (var wi = 0; wi < numSubWords; wi++)
                            {
                                var subWordStartIndex = wi * maxLineLength;
                                subWords.Add(words[i].Substring(subWordStartIndex, Math.Min(maxLineLength, words[i].Length - subWordStartIndex)));
                            }

                            words.RemoveAt(i);
                            words.InsertRange(i, subWords);

                            break;
                        }
                    }
                } while (foundLongWord);

                // Build our wrapped lines
                var currentLine = "";
                foreach (var word in words)
                {
                    if (currentLine.Length + word.Length + Math.Min(currentLine.Length, 1) <= maxLineLength)
                    {
                        if (!string.IsNullOrWhiteSpace(currentLine))
                            currentLine += " ";

                        currentLine += word;
                        continue;
                    }
                 
                    // Don't add the prefix to the first line
                    if (lines.Count == 0)
                        lines.Add(currentLine + lineSuffix);
                    else
                        lines.Add(linePrefix + currentLine + lineSuffix);

                    currentLine = word;
                }

                // Add our last line, but don't add the suffix to it
                if (!string.IsNullOrWhiteSpace(currentLine))
                    lines.Add(linePrefix + currentLine);
            }

            return lines;
        }
    }
}
