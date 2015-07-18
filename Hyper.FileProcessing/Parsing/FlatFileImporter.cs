using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Hyper.FileProcessing.Parsing
{
    public class FlatFileImporter
    {
        #region Public Methods

        public static FlatFileImportResult PerformImport(Stream inputStream, FlatFileTemplate objFileTemplate, DataColumnMapping columnMapping)
        {
            return PerformImport(new TextFieldParser(inputStream), objFileTemplate, columnMapping);
        }

        public static FlatFileImportResult PerformImport(string filepath, FlatFileTemplate objFileTemplate, DataColumnMapping columnMapping)
        {
            return PerformImport(new TextFieldParser(filepath), objFileTemplate, columnMapping);
        }

        #endregion Public Methods

        #region Private Methods

        private static FlatFileImportResult PerformImport(TextFieldParser objParser, FlatFileTemplate objFileTemplate, DataColumnMapping columnMapping)
        {
            FlatFileImportResult result = new FlatFileImportResult();

            objParser.SetDelimiters(objFileTemplate.Delimiters);
            objParser.SetFieldWidths(objFileTemplate.FieldWidths);
            objParser.HasFieldsEnclosedInQuotes = objFileTemplate.HasFieldsEnclosedInQuotes;
            objParser.TextFieldType = objFileTemplate.TextFieldType;
            objParser.TrimWhiteSpace = objFileTemplate.TrimWhiteSpace;

            var importAborted = false;
            var rawFieldValues = new List<string>();

            try
            {
                var mappedColumnCount = objFileTemplate.Columns.Count(columnMapping.ContainsSourceColumn);
                if (mappedColumnCount == 0)
                {
                    importAborted = true;
                    result.Errors.Add(new FlatFileImportException("The specified FeedFileColumnMapping object did not contain mappings for any of the columns defined in the specified FeedFileTemplate object."));
                }

                while (!objParser.EndOfData && !importAborted)
                {
                    try
                    {
                        var currentLineNumber = objParser.LineNumber;
                        if (currentLineNumber <= objFileTemplate.HeaderLinesToSkip)
                        {
                            result.HeaderLines.Add(objParser.ReadLine());
                            continue;
                        }

                        try
                        {
                            rawFieldValues.AddRange(objParser.ReadFields());
                        }
                        catch
                        {
                            // If we have a read error, we need to abort the read
                            importAborted = true;
                            throw;
                        }

                        result.TotalRecords++;

                        if (rawFieldValues.Count > objFileTemplate.Columns.Count)
                        {
                            result.SkippedRecords++;
                            throw new FlatFileFormatException("Line " + currentLineNumber + ": File has more fields than expected. Skipping line.");
                        }
                        else if (rawFieldValues.Count < objFileTemplate.Columns.Count)
                        {
                            result.SkippedRecords++;
                            throw new FlatFileFormatException("Line " + currentLineNumber + ": File has fewer fields than expected. Skipping line.");
                        }

                        var dicNamedImportValues = new Dictionary<string, string>();
                        for (var i = 0; i < objFileTemplate.Columns.Count; i++)
                        {
                            dicNamedImportValues.Add(objFileTemplate.Columns[i], rawFieldValues[i]);
                        }

                        var dicNamedDestinationValues = dicNamedImportValues.Keys.Where(columnMapping.ContainsSourceColumn).ToDictionary(columnMapping.GetDestinationColumn, sourceColumn => columnMapping.TransformValue(sourceColumn, dicNamedImportValues[sourceColumn]));

                        if (dicNamedDestinationValues.Count > 0)
                        {
                            result.ImportedData.Add(dicNamedDestinationValues);
                        }
                        else
                        {
                            result.Warnings.Add("Line " + currentLineNumber + ": No data imported because none of the columns were mapped. Skipping line.");
                            result.SkippedRecords++;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(ex);
                    }
                    finally
                    {
                        rawFieldValues.Clear();
                    }
                }
            }
            finally
            {
                if (objParser != null)
                {
                    objParser.Close();
                }
            }

            if (result.ImportedData.Count == 0)
            {
                var errorString = "No data was imported because ";
                if (importAborted)
                {
                    errorString += "the import was aborted.";
                }
                else if (result.TotalRecords == 0)
                {
                    errorString += "the file had no records.";
                }
                else if (result.SkippedRecords == result.TotalRecords)
                {
                    errorString += "all records were skipped.";
                }

                result.Errors.Add(new FlatFileImportException(errorString));
            }

            return result;
        }

        #endregion Private Methods
    }
}
