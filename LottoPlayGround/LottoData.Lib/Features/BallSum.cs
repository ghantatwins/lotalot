using System.Collections.Generic;
using Lotto.Combinatorics;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Factories;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Features;

namespace LottoData.Lib.Features
{
    public class BallSum:ISingleRowFeature
    {
        private readonly IFeaturesFactory _featuresManager;

        public BallSum() : this(new FeaturesFactory())
        {

        }

        public BallSum(IFeaturesFactory featuresManager)
        {
            _featuresManager = featuresManager;
        }

        public IData Extract(IData row)
        {
            return new FeatureData<long>(FeatureName, _featuresManager.SumOfBalls(row));
        }

        

        public string FeatureName
        {
            get
            {
                return FeatureNames.BallSum;
            }
        }
    }
    public class SumIndex : IOneOnOneFeature
    {
        private readonly IFeaturesFactory _featuresManager;
        private readonly int _factor;
        private readonly int _minSum;

        public SumIndex(ICombination<int> model) : this(model,new FeaturesFactory())
        {

        }

        public SumIndex(ICombination<int> model,IFeaturesFactory featuresManager)
        {
            var model1 = model;
            _featuresManager = featuresManager;
            _minSum = _featuresManager.SumOfBalls(_featuresManager.CreateDraw(model1.Element(0), model1));
            var maxSum = _featuresManager.SumOfBalls(_featuresManager.CreateDraw(model1.Element(model1.TotalCombinations - 1), model1));
            _factor = maxSum / _minSum;
        }

        public List<IData> Extract(IData row)
        {
            var sum = _featuresManager.SumOfBalls(row);
            return  _featuresManager.SumIndexes(_factor,sum,_minSum);
        }



        public string FeatureName
        {
            get
            {
                return FeatureNames.SumIndex;
            }
        }
    }
}
