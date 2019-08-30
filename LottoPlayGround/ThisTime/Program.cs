using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lotto.Combinatorics;
using LottoData.Lib;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Factories;
using LottoData.Lib.Interfaces;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;

namespace ThisTime
{
    class Program
    {
        static void Main(string[] args)
        {
            string resultsPath = @"C:\VSProjs\Data\ej.csv";

            IMultiLottoManager managerMain = new MultiLottoManager(resultsPath);
            IMultiLottoManager specMain = new FeaturesMultiLottoManager(managerMain);
            IList<IData> dataSet = specMain.Data;
            List<LottoAggregate> aggregates= new List<LottoAggregate>();
            for (int i = dataSet.Count-1; i >=0; i--)
            {
                LottoAggregate aggregate= new LottoAggregate();
                for (int j = 0; j < 3; j++)
                {
                    aggregate.Add(GetName(dataSet, i, j), GetData(dataSet, i, j));
                    
                }
                //aggregate.Print(Console.Write);
                aggregates.Add(aggregate);
                
            }

            MultiCombination<int> mainModel = (MultiCombination<int>) managerMain.Spec.CreateModel();

            Console.WriteLine("Average "+PrettyPrint(dataSet,mainModel,aggregates,0));
            Console.WriteLine("Average " + PrettyPrint(dataSet, mainModel.Main, aggregates, 1));
            Console.WriteLine("Average " + PrettyPrint(dataSet, mainModel.Sub, aggregates, 2));
        }

        static string PrettyPrint(IList<IData> dataSet,ICombination<int> mainModel, List<LottoAggregate> aggregates,int j)
        {
            return string.Format("{0} is {1}", GetName(dataSet, 0, j), string.Join(",",mainModel.Element((long) aggregates.Average(x => x.Data[GetName(dataSet, 0, j)]))));
        }

        private static long GetData(IList<IData> dataSet, int i,int j)
        {
            return ((FeatureData<long>)((BaseData<List<IData>>)dataSet[i]).Data[j]).Value;
        }
        private static string GetName(IList<IData> dataSet, int i,int j)
        {
            return ((FeatureData<long>)((BaseData<List<IData>>)dataSet[i]).Data[j]).FeatureName;
        }




    }

    public class LottoAggregate
    {
        public IDictionary<string, long> Data
        {
            get { return _collections; }
        }
        private Dictionary<string,long> _collections;

        public LottoAggregate()
        {
            _collections= new Dictionary<string, long>();
        }

        public void Add(string getName, long getData)
        {
            if (!_collections.ContainsKey(getName))
            {
                _collections.Add(getName, getData);
            }
            else
            _collections[getName] = getData;
        }

        
        public void Print(Action<string, object, object> writeLine)
        {
            foreach (var pair in _collections)
            {
                writeLine("{0}=>{1} ", pair.Key, pair.Value);
            }

        }

       

   
    }
}
