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
    public class LottoFeaturesDataFactory : ILotteryDataFactory
    {
        private readonly ILotteryDataFactory _factory;
        private readonly IEnumerable<IFeature> _features;

        public LottoFeaturesDataFactory(ILotteryDataFactory factory,IEnumerable<IFeature> features)
        {
            _factory = factory;
            _features = features;
        }

        

        public IEnumerable<IData> CreateDataRows(IDataSet dataSet)
        {
           
            var rows= _factory.CreateDataRows(dataSet).ToList();
            List<IData> tempSet = new List<IData>();
            Dictionary<string,List<IData>> extractions=new Dictionary<string, List<IData>>();
            foreach (var row in rows)
            {
                foreach (var feature in _features.OfType<ISingleRowFeature>())
                {
                    if (!extractions.ContainsKey(feature.FeatureName))
                    {
                        extractions.Add(feature.FeatureName,new List<IData>());
                    }
                    extractions[feature.FeatureName].Add(feature.Extract(row));
                }
                foreach (var feature in _features.OfType<IOneOnOneFeature>())
                {
                   foreach (var child in feature.Extract(row))
                   {
                      
                        string header = GetHeader(child);
                        if (!extractions.ContainsKey(header))
                        {
                            extractions.Add(header, new List<IData>());
                        }
                        extractions[header].Add(child);
                    }
                    
                }
                tempSet.Add(row);
                if (tempSet.Count >1)
                {
                    foreach (var feature in _features.OfType<IDualRowFeature>())
                    {
                        if (!extractions.ContainsKey(feature.FeatureName))
                        {
                            extractions.Add(feature.FeatureName, new List<IData>());
                        }

                        extractions[feature.FeatureName].Add(feature.Extract(tempSet));
                    }

                    tempSet.Remove(tempSet[0]);
                }
                
            }

            return CreateData(extractions);
        }

        public IList<IData> GetColumnData(IData data)
        {
            return ((BaseData<List<IData>>) data).Data;
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
                return AsFeatureData( data);
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
            return ((FeatureData<long>) data).FeatureName;
            return ((FeatureData<string>)data).FeatureName;
        }

        private bool IsFeatureData(IData data)
        {
            return data is FeatureData<long>|| data is FeatureData<string>;
        }

        private static BaseData<T> AsBaseData<T>(IData data)
        {
            return (data as BaseData<T>);
        }


        private IEnumerable<IData> CreateData(Dictionary<string, List<IData>> data)
        {
            List<IData> extractions=new List<IData>();
            int rows = data.First().Value.Count;
            for (int i = 0; i < rows; i++)
            {
                List<IData> columnValues=new List<IData>();
                foreach (var pair in data.Keys)
                {
                    columnValues.Add(data[pair][i]);
                }
                extractions.Add(new BaseData<List<IData>>(columnValues,"Collection"));
            }
            
            return extractions;
        }
        

        public ILottoConfig Config => _factory.Config;

        
    }
}