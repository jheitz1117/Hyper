using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hyper.BatchProcessing;

namespace Hyper.FileProcessing.Parsing
{
    public class HyperObjectMapper
    {
        #region Public Methods

        public static ObjectMappingResult<T> MapObjectData<T>(List<Dictionary<string, string>> objectData) where T : new()
        {
            var result = new ObjectMappingResult<T>();

            long recordNumber = 1;
            foreach (var lineData in objectData)
            {
                var objNewRecord = new T();

                var batchResult = PopulateObject(objNewRecord, lineData);
                if (batchResult.HadErrors || batchResult.HadWarnings)
                {
                    batchResult.BatchItemNumber = recordNumber;

                    result.HadWarnings |= batchResult.HadWarnings;
                    result.HadErrors |= batchResult.HadErrors;

                    result.BatchResults.Add(batchResult);
                    result.PartialItems.Add(objNewRecord);
                }

                result.TransformedItems.Add(objNewRecord);

                recordNumber++;
            }

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private static BatchResult PopulateObject(object objTarget, Dictionary<string, string> propertyValues)
        {
            var result = new BatchResult();

            if (objTarget == null || propertyValues == null || propertyValues.Count == 0)
            {
                return result;
            }

            var targetProperties = new List<PropertyInfo>(objTarget.GetType().GetProperties());
            foreach (var targetProperty in targetProperties.Where(targetProperty => propertyValues.ContainsKey(targetProperty.Name)))
            {
                try
                {
                    var rawValue = propertyValues[targetProperty.Name];
                    object convertedValue;

                    var typeToCheck = targetProperty.PropertyType;
                    if (targetProperty.PropertyType.IsGenericType && targetProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //If we have a nullable type and a blank raw value, we'll just set the property to null
                        if (string.IsNullOrWhiteSpace(rawValue))
                        {
                            targetProperty.SetValue(objTarget, null, null);
                            continue;
                        }

                        // If we get here, we actually want to check the underlying type
                        typeToCheck = Nullable.GetUnderlyingType(targetProperty.PropertyType);
                    }

                    if (typeToCheck == typeof(string))
                    {
                        convertedValue = rawValue;
                    }
                    else if (typeToCheck == typeof(short))
                    {
                        convertedValue = Convert.ToInt16(rawValue);
                    }
                    else if (typeToCheck == typeof(int))
                    {
                        convertedValue = Convert.ToInt32(rawValue);
                    }
                    else if (typeToCheck == typeof(long))
                    {
                        convertedValue = Convert.ToInt64(rawValue);
                    }
                    else if (typeToCheck == typeof(decimal))
                    {
                        convertedValue = Convert.ToDecimal(rawValue);
                    }
                    else if (typeToCheck == typeof(bool))
                    {
                        convertedValue = Convert.ToBoolean(rawValue);
                    }
                    else if (typeToCheck == typeof(byte))
                    {
                        convertedValue = Convert.ToByte(rawValue);
                    }
                    else if (typeToCheck == typeof(char))
                    {
                        convertedValue = Convert.ToChar(rawValue);
                    }
                    else if (typeToCheck == typeof(DateTime))
                    {
                        convertedValue = Convert.ToDateTime(rawValue);
                    }
                    else if (typeToCheck == typeof(double))
                    {
                        convertedValue = Convert.ToDouble(rawValue);
                    }
                    else if (typeToCheck == typeof(sbyte))
                    {
                        convertedValue = Convert.ToSByte(rawValue);
                    }
                    else if (typeToCheck == typeof(float))
                    {
                        convertedValue = Convert.ToSingle(rawValue);
                    }
                    else if (typeToCheck == typeof(ushort))
                    {
                        convertedValue = Convert.ToUInt16(rawValue);
                    }
                    else if (typeToCheck == typeof(uint))
                    {
                        convertedValue = Convert.ToUInt32(rawValue);
                    }
                    else if (typeToCheck == typeof(ulong))
                    {
                        convertedValue = Convert.ToUInt64(rawValue);
                    }
                    else if (typeToCheck.IsEnum)
                    {
                        convertedValue = Convert.ChangeType(rawValue, Enum.GetUnderlyingType(typeToCheck));

                        if (Enum.IsDefined(typeToCheck, convertedValue))
                            convertedValue = Enum.ToObject(typeToCheck, convertedValue);
                        else
                            throw new ObjectTransformationException("The enumeration '" + typeToCheck + "' does not contain the value '" + convertedValue + "'. Skipping this property.");
                    }
                    else
                    {
                        throw new NotSupportedException("The type '" + typeToCheck + "' is not currently supported.");
                    }

                    targetProperty.SetValue(objTarget, convertedValue, null);
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ObjectTransformationException("Failed to set property '" + targetProperty.Name + "' on object of type '" + objTarget.GetType() + "' to the value '" + propertyValues[targetProperty.Name] + "'.", ex));
                }
            }

            return result;
        }

        #endregion Private Methods
    }
}
