using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LottoData.Lib.Interfaces.DataSets
{
    public interface IDataSet
    {
        /// <summary>
        /// Headers or variables which contain decimal data
        /// </summary>
        IEnumerable<string> NumberVariables { get; }
        /// <summary>
        /// Total number of rows
        /// </summary>
        int Rows { get; }
        /// <summary>
        /// All variable or header names in file
        /// </summary>
        IEnumerable<string> VariableNames { get; }
        /// <summary>
        /// Number of columns
        /// </summary>
        int Columns { get; }

        IList<string> InternalVariableNames { get; }

        Dictionary<string, IList> VariableValues { get; }

        /// <summary>
        /// Get value from csv file by using row & header index
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        string GetValue(int rowIndex, int columnIndex);
        /// <summary>
        /// Get double representation of values for a given column
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        IEnumerable<double> GetDoubleValues(string variableName);
        IEnumerable<bool> GetBoolValues(string variableName);
        /// <summary>
        /// Get string representation of values for a given column
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        IEnumerable<string> GetStringValues(string variableName);
        /// <summary>
        /// Get date time representation of values for a given column
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        IEnumerable<DateTime> GetDateTimeValues(string variableName);
        /// <summary>
        /// Get read only double representation of values for a given column
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        ReadOnlyCollection<double> GetReadOnlyDoubleValues(string variableName);
        /// <summary>
        /// Get double representation of specific column
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        double GetDoubleValue(string variableName, int row);
        /// <summary>
        /// Get double representation of values for a given rows for a given specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        IEnumerable<double> GetDoubleValues(string variableName, IEnumerable<int> rows);
        /// <summary>
        /// Get generic representation of values for a given column and given specific rows
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableName"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        IEnumerable<T> GetValues<T>(string variableName, IEnumerable<int> rows);
        /// <summary>
        /// Get generic representation of values for a given column and all rows
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableName"></param>
        /// <returns></returns>
        List<T> GetValues<T>(string variableName);
        /// <summary>
        /// Check if variable of specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableName"></param>
        /// <returns></returns>
        bool VariableHasType<T>(string variableName);
    }
}