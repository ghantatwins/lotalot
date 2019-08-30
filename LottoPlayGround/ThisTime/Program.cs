using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AvlTree;
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
            string resultsPath = @"C:\VsProjs\lotalot\ej.csv";

            IMultiLottoManager managerMain = new MultiLottoManager(resultsPath);
            IMultiLottoManager specMain = new FeaturesMultiLottoManager(managerMain);
            IList<IData> dataSet = specMain.Data;
            AvlTree<LottoAggregate> tree= new AvlTree<LottoAggregate>();
            List<LottoAggregate> aggregates = new List<LottoAggregate>();
            string[] names = new string[3];
            for (int i = dataSet.Count - 1; i >= 0; i--)
            {
                LottoAggregate aggregate = new LottoAggregate();
                for (int j = 0; j < 3; j++)
                {
                    names[j] = GetName(dataSet, i, j);
                    aggregate.Add(names[j], GetData(dataSet, i, j));

                }
                tree.Insert(aggregate);
                aggregates.Add(aggregate);
                //tree.Print();
            }

            MultiCombination<int> mainModel = (MultiCombination<int>)managerMain.Spec.CreateModel();
            LottoAggregate findAggregate = new LottoAggregate();
            findAggregate.Add(names[0], GetIndex(dataSet,  aggregates, 0));
            findAggregate.Add(names[1], GetIndex(dataSet, aggregates, 1));
            findAggregate.Add(names[2], GetIndex(dataSet,  aggregates, 2));
            //tree.Print();
            var nearAggregate = tree.Find(aggregates[0]);
            
            Print(nearAggregate.left?.value,mainModel,names[0]);
            Print(nearAggregate.left?.value, mainModel.Main, names[1]);
            Print(nearAggregate.left?.value, mainModel.Sub, names[2]);
            
            Print(nearAggregate.right?.value, mainModel, names[0]);
            Print(nearAggregate.right?.value, mainModel.Main, names[1]);
            Print(nearAggregate.right?.value, mainModel.Sub, names[2]);

            Print(nearAggregate.parent?.value, mainModel, names[0]);
            Print(nearAggregate.parent?.value, mainModel.Main, names[1]);
            Print(nearAggregate.parent?.value, mainModel.Sub, names[2]);
            Console.Read();
        }

        private static void Print(LottoAggregate leftValue, ICombination<int> mainModel,string name)
        {
            Console.WriteLine(string.Join(",",mainModel.Element(leftValue.Data[name])));
        }

        static long GetIndex(IList<IData> dataSet, List<LottoAggregate> aggregates, int j)
        {
            return (long)aggregates.Average(x => x.Data[GetName(dataSet, 0, j)]);
        }

        private static long GetData(IList<IData> dataSet, int i, int j)
        {
            return ((FeatureData<long>)((BaseData<List<IData>>)dataSet[i]).Data[j]).Value;
        }
        private static string GetName(IList<IData> dataSet, int i, int j)
        {
            return ((FeatureData<long>)((BaseData<List<IData>>)dataSet[i]).Data[j]).FeatureName;
        }




    }

    public class LottoAggregate:IComparable<LottoAggregate>,IEquatable<LottoAggregate>
    {
        public IDictionary<string, long> Data
        {
            get { return _collections; }
        }
        private Dictionary<string, long> _collections;

        public LottoAggregate()
        {
            _collections = new Dictionary<string, long>();
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


        public void Print(Action<string> writeLine)
        {

            writeLine(ToString());
        }


        public int CompareTo(LottoAggregate other)
        {
            if (other == null) return 1;
            foreach (var collection in _collections)
            {
                if (other._collections.ContainsKey(collection.Key))
                {
                    return collection.Value.CompareTo(other._collections[collection.Key]);
                }
            }

            return 0;
        }

        public bool Equals(LottoAggregate other)
        {
            if (other == null) return false;
            return _collections.SequenceEqual(other._collections);
        }

        public override string ToString()
        {
            StringBuilder returnValue=new StringBuilder();
            //foreach (var pair in _collections)
            {
                returnValue.Append(string.Format("{0} ", _collections.First().Value));
            }

            return returnValue.ToString();
        }
    }
}
