using System;
using System.Collections.Generic;
using System.Linq;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Interfaces.DataSets;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Features;
using LottoData.Lib.Interfaces.Providers.Lottery;

namespace LottoData.Lib.Factories
{
    public class FiboBinFactory : ILotteryDataFactory
    {
        private readonly ILotteryDataFactory _factory;
        private readonly ILotterySpec _spec;


        public FiboBinFactory(ILotteryDataFactory factory,ILotterySpec spec)
        {
            _factory = factory;
            _spec = spec;
        }

        public IEnumerable<IData> CreateDataRows(IDataSet dataSet)
        {
            List<IData> rows = new List<IData>();

            List<IFeature> features = new List<IFeature>(_spec.Features);

            for (int j = 0; j < dataSet.Rows; j++)
            {
                List<BaseData<string>> data = new List<BaseData<string>>();

               
                
                for (int i = 0; i < features.Count; i++)
                {

                    data.Add(new BaseData<string>(dataSet.GetValue(j, i), features[i].FeatureName));
                }
                rows.Add(new BaseData<IList<BaseData<string>>>(data, "RowVals"));

            }


            return rows;
        }

        public IList<IData> GetColumnData(IData data)
        {
            var subData = ((BaseData<IList<BaseData<string>>>)data).Data;
            return subData.Select(x => x as IData).ToList();
        }


        public IEnumerable<string> GetVariables(IList<IData> instance)
        {
            IData row = instance[0];
            IList<IData> columnData = GetColumnData(row);
            return columnData.Select(GetHeader);
        }

        public string GetData(IData data)
        {
            return _factory.GetData(data);
        }

        private string GetHeader(IData data)
        {

            if (IsFeatureData(data))
            {
                return AsFeatureData(data);
            }

            if (data is BaseData<IData>)
            {
                return AsBaseData<IData>(data).Field;
            }

            if (data is BaseData<int>)
            {
                return AsBaseData<int>(data).Field;
            }

            if (data is BaseData<double>)
            {
                return AsBaseData<double>(data).Field;
            }

            if (data is BaseData<DateTime>)
            {
                return AsBaseData<DateTime>(data).Field;
            }

            return AsBaseData<string>(data).Field;
        }
        private string AsFeatureData(IData data)
        {
            if (data is FeatureData<long>)
                return ((FeatureData<long>)data).FeatureName;
            return ((FeatureData<string>)data).FeatureName;
        }

        private bool IsFeatureData(IData data)
        {
            return data is FeatureData<long> || data is FeatureData<string>;
        }

        private static BaseData<T> AsBaseData<T>(IData data)
        {
            return (data as BaseData<T>);
        }


        private IEnumerable<IData> CreateData(Dictionary<string, List<IData>> data)
        {
            List<IData> extractions = new List<IData>();
            int rows = data.First().Value.Count;
            for (int i = 0; i < rows; i++)
            {
                List<IData> columnValues = new List<IData>();
                foreach (var pair in data.Keys)
                {
                    columnValues.Add(data[pair][i]);
                }

                extractions.Add(new BaseData<List<IData>>(columnValues, "Collection"));
            }

            return extractions;
        }


        public ILottoConfig Config => _factory.Config;

        
    }
}
