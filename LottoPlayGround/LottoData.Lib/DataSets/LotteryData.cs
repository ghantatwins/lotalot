using System;
using System.Collections.Generic;
using System.Linq;
using LottoData.Lib.Interfaces.DataSets;

namespace LottoData.Lib.DataSets
{
    public class LotteryData : DataSet
    {
        public LotteryData(IDataSet dataset) : this(dataset, dataset.NumberVariables)
        {

        }

        public LotteryData(IDataSet dataset, IEnumerable<string> allowedInputVariables):base(dataset)
        {
            List<string> allowedInputVars = allowedInputVariables?.ToList();
            if (dataset == null) throw new NullReferenceException("The dataset must not be null.");
            if (allowedInputVariables == null)
                throw new NullReferenceException("The allowedInputVariables must not be null.");

            if (allowedInputVars.Except(dataset.NumberVariables).Any())
                throw new ArgumentException(
                    "All allowed input variables must be present in the dataset and of type double.");

            InputVariables = dataset.NumberVariables.Where(allowedInputVars.Contains);
            DataSet = dataset;
            AllowedInputVariables = allowedInputVars;
            OutputVariables = dataset.NumberVariables.Select(x => x).Where(y => !allowedInputVars.Contains(y));

        }

        #region properties

        public virtual bool IsEmpty { get; protected set; }

        public IDataSet DataSet { get; }

        public IEnumerable<string> InputVariables { get; }
        public IEnumerable<string> OutputVariables { get; }
        public IEnumerable<string> AllowedInputVariables { get; }


        public virtual IEnumerable<int> AllIndices => Enumerable.Range(0, DataSet.Rows);

        #endregion
    }
}