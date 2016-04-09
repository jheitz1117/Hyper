using System.Linq;
using System;
using System.Collections.Generic;

namespace Hyper.FileProcessing.Parsing
{
    public class DataColumnMapping
    {
        #region Properties

        private Dictionary<string, string> SourceToDestination { get; } = new Dictionary<string, string>();

        private Dictionary<string, string> DestinationToSource { get; } = new Dictionary<string, string>();

        private Dictionary<string, List<DataColumnTransform>> ColumnTransforms { get; } = new Dictionary<string, List<DataColumnTransform>>();

        public List<string> SourceColumns => new List<string>(SourceToDestination.Keys);

        public List<string> DestinationColumns => new List<string>(DestinationToSource.Keys);

        #endregion Properties

        #region Public Methods

        public void Add(string sourceColumn, string destinationColumn)
        {
            Add(sourceColumn, destinationColumn, null);
        }

        public void Add(string sourceColumn, string destinationColumn, ColumnTransformDelegate columnTransform)
        {
            if (string.IsNullOrWhiteSpace(sourceColumn))
                throw new ArgumentNullException(nameof(sourceColumn), "Destination column '" + destinationColumn + "' must have a corresponding source column.");
            if (string.IsNullOrWhiteSpace(sourceColumn))
                throw new ArgumentNullException(nameof(destinationColumn), "Source column '" + sourceColumn + "' must have a corresponding destination column.");

            if (!ContainsSourceColumn(sourceColumn) && !ContainsDestinationColumn(destinationColumn))
            {
                SourceToDestination.Add(sourceColumn, destinationColumn);
                DestinationToSource.Add(destinationColumn, sourceColumn);
            }
            else if (ContainsSourceColumn(sourceColumn) && GetDestinationColumn(sourceColumn) != destinationColumn)
                throw new ArgumentException("The source column '" + sourceColumn + "' cannot be mapped to destination column '" + destinationColumn + "' because it has already been mapped to the column '" + GetDestinationColumn(sourceColumn) + "'.");
            else if (ContainsDestinationColumn(destinationColumn) && GetSourceColumn(destinationColumn) != sourceColumn)
                throw new ArgumentException("The destination column '" + destinationColumn + "' cannot be mapped to source column '" + sourceColumn + "' because it has already been mapped to the column '" + GetSourceColumn(destinationColumn) + "'.");

            if (columnTransform != null)
                AddColumnTransform(sourceColumn, columnTransform);
        }

        public void RemoveSourceColumn(string sourceColumn)
        {
            ClearColumnTransforms(sourceColumn);
            DestinationToSource.Remove(SourceToDestination[sourceColumn]);
            SourceToDestination.Remove(sourceColumn);
        }

        public void RemoveDestinationColumn(string destinationColumn)
        {
            ClearColumnTransforms(DestinationToSource[destinationColumn]);
            SourceToDestination.Remove(DestinationToSource[destinationColumn]);
            DestinationToSource.Remove(destinationColumn);
        }

        public string GetDestinationColumn(string sourceColumn)
        {
            return SourceToDestination[sourceColumn];
        }

        public string GetSourceColumn(string destinationColumn)
        {
            return DestinationToSource[destinationColumn];
        }

        public bool ContainsSourceColumn(string sourceColumn)
        {
            return (SourceToDestination.ContainsKey(sourceColumn) && DestinationToSource.ContainsValue(sourceColumn));
        }

        public bool ContainsDestinationColumn(string destinationColumn)
        {
            return (DestinationToSource.ContainsKey(destinationColumn) && SourceToDestination.ContainsValue(destinationColumn));
        }

        public void Clear()
        {
            SourceToDestination.Clear();
            DestinationToSource.Clear();
            ColumnTransforms.Clear();
        }

        public bool ColumnHasTransforms(string sourceColumn)
        {
            return ColumnTransforms.ContainsKey(sourceColumn);
        }

        public string TransformValue(string sourceColumn, string sourceValue)
        {
            var transformedValue = sourceValue;

            if (ColumnHasTransforms(sourceColumn))
                transformedValue = ColumnTransforms[sourceColumn].OrderBy(x => x.ExecutionOrder).Aggregate(transformedValue, (current, transform) => transform.TransformValue(current));

            return transformedValue;
        }

        /// <summary>
        /// Adds the specified column transform to the end of the transform list for the specified source column
        /// </summary>
        /// <param name="sourceColumn">The name of the source column for which to add the column transform</param>
        /// <param name="columnTransform">The transform delegate to add</param>
        public void AddColumnTransform(string sourceColumn, ColumnTransformDelegate columnTransform)
        {
            AddColumnTransform(sourceColumn, columnTransform, GetMaxExecutionOrder(sourceColumn) + 1);
        }

        /// <summary>
        /// Adds the specified column transform to the transform list at the specified location for the specified source column 
        /// </summary>
        /// <param name="sourceColumn">The name of the source column for which to add the column transform</param>
        /// <param name="columnTransform">The transform delegate to add</param>
        /// <param name="executionOrder">The order of execution for this transform</param>
        public void AddColumnTransform(string sourceColumn, ColumnTransformDelegate columnTransform, int executionOrder)
        {
            AddColumnTransform(sourceColumn, new DataColumnTransform(columnTransform, executionOrder));
        }

        /// <summary>
        /// Adds the specified column transform to the end of the transform list for the specified source column
        /// </summary>
        /// <param name="sourceColumn">The name of the source column for which to add the column transform</param>
        /// <param name="columnTransform">The transform delegate to add</param>
        /// <param name="parameter">The parameter for the transform delegate</param>
        public void AddColumnTransform<T>(string sourceColumn, ParameterizedColumnTransformDelegate<T> columnTransform, T parameter)
        {
            AddColumnTransform(sourceColumn, columnTransform, parameter, GetMaxExecutionOrder(sourceColumn) + 1);
        }

        /// <summary>
        /// Adds the specified column transform to the transform list at the specified location for the specified source column 
        /// </summary>
        /// <param name="sourceColumn">The name of the source column for which to add the column transform</param>
        /// <param name="columnTransform">The transform delegate to add</param>
        /// <param name="parameter">The parameter for the transform delegate</param>
        /// <param name="executionOrder">The order of execution for this transform</param>
        public void AddColumnTransform<T>(string sourceColumn, ParameterizedColumnTransformDelegate<T> columnTransform, T parameter, int executionOrder)
        {
            AddColumnTransform(sourceColumn, new ParameterizedDataColumnTransform<T>(columnTransform, parameter, executionOrder));
        }

        /// <summary>
        /// Clears all column transforms for the specified column
        /// </summary>
        /// <param name="sourceColumn">The name of the column for which to clear transforms</param>
        public void ClearColumnTransforms(string sourceColumn)
        {
            if (ColumnHasTransforms(sourceColumn))
            {
                ColumnTransforms.Remove(sourceColumn);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private int GetMaxExecutionOrder(string sourceColumn)
        {
            if (!ContainsSourceColumn(sourceColumn))
                throw new KeyNotFoundException("Column mapping does not contain the source column '" + sourceColumn + "'.");

            var executionOrder = 0;

            if (ColumnHasTransforms(sourceColumn) && ColumnTransforms[sourceColumn] != null)
                executionOrder = ColumnTransforms[sourceColumn].Select(x => x.ExecutionOrder).Max();

            return executionOrder;
        }

        private void AddColumnTransform(string sourceColumn, DataColumnTransform transform)
        {
            if (!ContainsSourceColumn(sourceColumn))
                throw new KeyNotFoundException("Column mapping does not contain the source column '" + sourceColumn + "'.");

            if (!ColumnHasTransforms(sourceColumn) || ColumnTransforms[sourceColumn] == null)
                ColumnTransforms[sourceColumn] = new List<DataColumnTransform>();

            ColumnTransforms[sourceColumn].Add(transform);
        }

        #endregion Private Methods
    }
}