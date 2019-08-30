using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LottoData.Lib.Interfaces.DataSets;

namespace LottoData.Lib.DataSets
{
    public class DataSet : IDataSet
    {
        private List<string> _internalVariableNames;

        private readonly Dictionary<string, IList> _variableValues;


        public DataSet(ICollection<string> variableNames, ICollection<IList> variableValues)
        {

            if (!variableNames.Any())
            {
                _internalVariableNames = Enumerable.Range(0, variableValues.Count()).Select(x => "Column " + x).ToList();
            }
            else if (variableNames.Count() != variableValues.Count())
            {
                throw new ArgumentException(
                    "Number of variable names doesn't match the number of columns of variableValues");
            }
            else if (variableValues.Any(list => list.Count != variableValues.First().Count))
            {
                throw new ArgumentException("The number of values must be equal for every variable");
            }
            else if (variableNames.Distinct().Count() != variableNames.Count())
            {
                var duplicateVariableNames =
                    variableNames.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                var message = "The dataset cannot contain duplicate variables names: " + Environment.NewLine;
                message = duplicateVariableNames.Aggregate(message, (current, duplicateVariableName) => current + (duplicateVariableName + Environment.NewLine));
                throw new ArgumentException(message);
            }

            Rows = variableValues.First().Count;
            _internalVariableNames = new List<string>(variableNames);
            _variableValues = new Dictionary<string, IList>(_internalVariableNames.Count);
            for (var i = 0; i < _internalVariableNames.Count; i++)
            {
                var values = variableValues.ElementAt(i);
                _variableValues.Add(_internalVariableNames[i], values);
            }
        }

        public DataSet(ICollection<string> variableNames, double[,] variableValues)
        {

            if (variableNames.Count() != variableValues.GetLength(1))
            {
                throw new ArgumentException(
                    "Number of variable names doesn't match the number of columns of variableValues");
            }
            if (variableNames.Distinct().Count() != variableNames.Count())
            {
                var duplicateVariableNames =
                    variableNames.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                var message = "The dataset cannot contain duplicate variables names: " + Environment.NewLine;
                message = duplicateVariableNames.Aggregate(message, (current, duplicateVariableName) => current + (duplicateVariableName + Environment.NewLine));
                throw new ArgumentException(message);
            }

            Rows = variableValues.GetLength(0);
            _internalVariableNames = new List<string>(variableNames);

            _variableValues = new Dictionary<string, IList>(variableValues.GetLength(1));
            for (var col = 0; col < variableValues.GetLength(1); col++)
            {
                var columName = _internalVariableNames[col];
                var values = new List<double>(variableValues.GetLength(0));
                for (var row = 0; row < variableValues.GetLength(0); row++)
                {
                    values.Add(variableValues[row, col]);
                }
                _variableValues.Add(columName, values);
            }
        }

        protected DataSet(IDataSet dataset) : this(dataset.InternalVariableNames, dataset.VariableValues.Values)
        {
        }



        public IEnumerable<string> VariableNames
        {
            get { return _internalVariableNames; }
            protected set
            {
                if (_internalVariableNames != null) throw new InvalidOperationException();
                _internalVariableNames = new List<string>(value);
            }
        }


        public Dictionary<string, IList> VariableValues
        {
            get
            {
                return _variableValues;
            }
        }

        public string GetValue(int rowIndex, int columnIndex)
        {
            return _variableValues[_internalVariableNames[columnIndex]][rowIndex].ToString();
        }



        public IEnumerable<string> NumberVariables
        {
            get { return _variableValues.Where(p => p.Value is List<double>).Select(p => p.Key); }

        }

        public IEnumerable<double> GetDoubleValues(string variableName)
        {
            return GetValues<double>(variableName);
        }

        public IEnumerable<bool> GetBoolValues(string variableName)
        {
            return GetValues<bool>(variableName);
        }

        public IEnumerable<string> GetStringValues(string variableName)
        {
            return GetValues<string>(variableName);
        }

        public IEnumerable<DateTime> GetDateTimeValues(string variableName)
        {
            return GetValues<DateTime>(variableName);
        }

        public ReadOnlyCollection<double> GetReadOnlyDoubleValues(string variableName)
        {
            var values = GetValues<double>(variableName);
            return values.AsReadOnly();
        }

        public double GetDoubleValue(string variableName, int row)
        {
            var values = GetValues<double>(variableName);
            return values[row];
        }

        public IEnumerable<double> GetDoubleValues(string variableName, IEnumerable<int> rows)
        {
            return GetValues<double>(variableName, rows);
        }

        public IEnumerable<T> GetValues<T>(string variableName, IEnumerable<int> rows)
        {
            var values = GetValues<T>(variableName);
            return rows.Select(x => values[x]);
        }

        public List<T> GetValues<T>(string variableName)
        {
            IList list;
            if (!_variableValues.TryGetValue(variableName, out list))
                throw new ArgumentException("The variable " + variableName + " does not exist in the dataset.");
            var values = list as List<T>;
            if (values == null)
            {
                values = GetCastedValues<T>(list);
                if (values == null)
                    throw new ArgumentException("The variable " + variableName + " is not a " + typeof(T) + " variable.");
            }
            return values;
        }

        private List<T> GetCastedValues<T>(IList list)
        {
            List<T> castedValues = new List<T>();
            foreach (var value in list)
            {
                try
                {
                    T castedValue = (T)Convert.ChangeType(value, typeof(T));
                    castedValues.Add(castedValue);
                }
                catch (Exception)
                {
                    return null;
                }


            }
            return castedValues;
        }

        public bool VariableHasType<T>(string variableName)
        {
            return _variableValues[variableName] is IList<T>;
        }

        public int Rows { get; protected set; }

        public int Columns => _internalVariableNames.Count;

        public IList<string> InternalVariableNames
        {
            get
            {
                return _internalVariableNames;
            }
        }
    }
}