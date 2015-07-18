using System.Linq;
using System;
using System.Collections.Generic;

namespace Hyper.FileProcessing.Parsing
{
    public class DataColumnMapping
    {
        #region Properties

        private Dictionary<string, string> _sourceToDestination = new Dictionary<string, string>();
        private Dictionary<string, string> SourceToDestination
        {
            get
            {
                return _sourceToDestination;
            }
            set
            {
                _sourceToDestination = value;
            }
        }

        private Dictionary<string, string> _destinationToSource = new Dictionary<string, string>();
        private Dictionary<string, string> DestinationToSource
        {
            get
            {
                return _destinationToSource;
            }
            set
            {
                _destinationToSource = value;
            }
        }

        private Dictionary<string, List<DataColumnTransform>> _columnTransforms = new Dictionary<string, List<DataColumnTransform>>();
        private Dictionary<string, List<DataColumnTransform>> ColumnTransforms
        {
            get
            {
                return _columnTransforms;
            }
            set
            {
                _columnTransforms = value;
            }
        }

        public List<string> SourceColumns
        {
            get
            {
                return new List<string>(SourceToDestination.Keys);
            }
        }

        public List<string> DestinationColumns
        {
            get
            {
                return new List<string>(DestinationToSource.Keys);
            }
        }

        #endregion Properties

        #region Public Methods

        public void Add(string sourceColumn, string destinationColumn)
        {
            Add(sourceColumn, destinationColumn, null);
        } // end Add()

        public void Add(string sourceColumn, string destinationColumn, ColumnTransformDelegate columnTransform)
        {
            if (string.IsNullOrWhiteSpace(sourceColumn) && string.IsNullOrWhiteSpace(destinationColumn))
            {
                throw new ArgumentNullException("Both the source and destination column names must be non-blank.");
            }
            else if (string.IsNullOrWhiteSpace(sourceColumn) && !string.IsNullOrWhiteSpace(destinationColumn))
            {
                throw new ArgumentNullException("sourceColumn", "Destination column '" + destinationColumn + "' must have a corresponding source column.");
            }
            else if (!string.IsNullOrWhiteSpace(sourceColumn) && string.IsNullOrWhiteSpace(destinationColumn))
            {
                throw new ArgumentNullException("destinationColumn", "Source column '" + sourceColumn + "' must have a corresponding destination column.");
            }

            if (!ContainsSourceColumn(sourceColumn) && !ContainsDestinationColumn(destinationColumn))
            {
                SourceToDestination.Add(sourceColumn, destinationColumn);
                DestinationToSource.Add(destinationColumn, sourceColumn);
            }
            else if (ContainsSourceColumn(sourceColumn) && GetDestinationColumn(sourceColumn) != destinationColumn)
            {
                throw new ArgumentException("The source column '" + sourceColumn + "' cannot be mapped to destination column '" + destinationColumn + "' because it has already been mapped to the column '" + GetDestinationColumn(sourceColumn) + "'.");
            }
            else if (ContainsDestinationColumn(destinationColumn) && GetSourceColumn(destinationColumn) != sourceColumn)
            {
                throw new ArgumentException("The destination column '" + destinationColumn + "' cannot be mapped to source column '" + sourceColumn + "' because it has already been mapped to the column '" + GetSourceColumn(destinationColumn) + "'.");
            }

            if (columnTransform != null)
            {
                AddColumnTransform(sourceColumn, columnTransform);
            }
        } // end Add()

        public void RemoveSourceColumn(string sourceColumn)
        {
            ClearColumnTransforms(sourceColumn);
            DestinationToSource.Remove(SourceToDestination[sourceColumn]);
            SourceToDestination.Remove(sourceColumn);
        } // end RemoveSourceColumn()

        public void RemoveDestinationColumn(string destinationColumn)
        {
            ClearColumnTransforms(DestinationToSource[destinationColumn]);
            SourceToDestination.Remove(DestinationToSource[destinationColumn]);
            DestinationToSource.Remove(destinationColumn);
        } // end RemoveSourceColumn()

        public string GetDestinationColumn(string sourceColumn)
        {
            return SourceToDestination[sourceColumn];
        } // end GetDestinationColumn()

        public string GetSourceColumn(string destinationColumn)
        {
            return DestinationToSource[destinationColumn];
        } // end GetSourceColumn()

        public bool ContainsSourceColumn(string sourceColumn)
        {
            return (SourceToDestination.ContainsKey(sourceColumn) && DestinationToSource.ContainsValue(sourceColumn));
        } // end ContainsSourceColumn()

        public bool ContainsDestinationColumn(string destinationColumn)
        {
            return (DestinationToSource.ContainsKey(destinationColumn) && SourceToDestination.ContainsValue(destinationColumn));
        } // end ContainsDestinationColumn()

        public void Clear()
        {
            SourceToDestination.Clear();
            DestinationToSource.Clear();
            ColumnTransforms.Clear();
        } // end Clear()

        public bool ColumnHasTransforms(string sourceColumn)
        {
            return ColumnTransforms.ContainsKey(sourceColumn);
        } // end ColumnHasTransforms()

        public string TransformValue(string sourceColumn, string sourceValue)
        {
            string transformedValue = sourceValue;

            if (ColumnHasTransforms(sourceColumn))
            {
                foreach (DataColumnTransform transform in ColumnTransforms[sourceColumn].OrderBy(x => x.ExecutionOrder))
                {
                    transformedValue = transform.TransformValue(transformedValue);
                }
            }

            return transformedValue;
        } // end TransformValue()

        /// <summary>
        /// Adds the specified column transform to the end of the transform list for the specified source column
        /// </summary>
        /// <param name="sourceColumn">The name of the source column for which to add the column transform</param>
        /// <param name="columnTransform">The transform delegate to add</param>
        public void AddColumnTransform(string sourceColumn, ColumnTransformDelegate columnTransform)
        {
            AddColumnTransform(sourceColumn, columnTransform, GetMaxExecutionOrder(sourceColumn) + 1);
        } // end AddColumnTransform()

        /// <summary>
        /// Adds the specified column transform to the transform list at the specified location for the specified source column 
        /// </summary>
        /// <param name="sourceColumn">The name of the source column for which to add the column transform</param>
        /// <param name="columnTransform">The transform delegate to add</param>
        /// <param name="executionOrder">The order of execution for this transform</param>
        public void AddColumnTransform(string sourceColumn, ColumnTransformDelegate columnTransform, int executionOrder)
        {
            AddColumnTransform(sourceColumn, new DataColumnTransform(columnTransform, executionOrder));
        } // end AddColumnTransform()

        /// <summary>
        /// Adds the specified column transform to the end of the transform list for the specified source column
        /// </summary>
        /// <param name="sourceColumn">The name of the source column for which to add the column transform</param>
        /// <param name="columnTransform">The transform delegate to add</param>
        /// <param name="parameter">The parameter for the transform delegate</param>
        public void AddColumnTransform<T>(string sourceColumn, ParameterizedColumnTransformDelegate<T> columnTransform, T parameter)
        {
            AddColumnTransform<T>(sourceColumn, columnTransform, parameter, GetMaxExecutionOrder(sourceColumn) + 1);
        } // end AddColumnTransform()

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
        } // end AddColumnTransform()

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
        } // end ClearColumnTransforms()

        #endregion Public Methods

        #region Private Methods

        private int GetMaxExecutionOrder(string sourceColumn)
        {
            if (!ContainsSourceColumn(sourceColumn))
            {
                throw new KeyNotFoundException("Column mapping does not contain the source column '" + sourceColumn + "'.");
            }

            int executionOrder = 0;

            if (ColumnHasTransforms(sourceColumn) && ColumnTransforms[sourceColumn] != null)
            {
                executionOrder = ColumnTransforms[sourceColumn].Select(x => x.ExecutionOrder).Max();
            }

            return executionOrder;
        } // end GetMaxExecutionOrder()

        private void AddColumnTransform(string sourceColumn, DataColumnTransform transform)
        {
            if (!ContainsSourceColumn(sourceColumn))
            {
                throw new KeyNotFoundException("Column mapping does not contain the source column '" + sourceColumn + "'.");
            }

            if (!ColumnHasTransforms(sourceColumn) || ColumnTransforms[sourceColumn] == null)
            {
                ColumnTransforms[sourceColumn] = new List<DataColumnTransform>();
            }

            ColumnTransforms[sourceColumn].Add(transform);
        } // end AddColumnTransform()

        #endregion Private Methods
    }
}