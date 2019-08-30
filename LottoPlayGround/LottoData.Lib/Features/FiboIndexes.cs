using System.Collections.Generic;
using System.Linq;
using Lotto.Combinatorics;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Factories;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Features;

namespace LottoData.Lib.Features
{
    public class FiboIndexes :  IOneOnOneFeature
    {
        private readonly ICombination<int> _model;

        private IFeaturesFactory _featuresManager;
        public FiboIndexes(ICombination<int> model) : this(model, new FeaturesFactory())
        {

        }


        public FiboIndexes(ICombination<int> model, IFeaturesFactory featuresManager)
        {
            _featuresManager = featuresManager;
            _model = model;
        }
        public string FeatureName
        {
            get { return FeatureNames.FiboIndexes; }
        }

        public List<IData> Extract(IData tempSet)
        {
            return _featuresManager.ToFibIndexs(((IDraw)tempSet).OfType<BaseData<int>>().ToArray(),_model);
        }

        

    }

    public class PatternMatcher : IOneOnOneFeature
    {
        private readonly ICombination<int> _model;

        private IFeaturesFactory _featuresManager;
        public PatternMatcher(ICombination<int> model) : this(model, new FeaturesFactory())
        {

        }


        public PatternMatcher(ICombination<int> model, IFeaturesFactory featuresManager)
        {
            _featuresManager = featuresManager;
            _model = model;
        }
        public string FeatureName
        {
            get { return FeatureNames.FiboIndexes; }
        }

        public List<IData> Extract(IData tempSet)
        {
            return _featuresManager.ToPatternMatches(((IDraw)tempSet).OfType<BaseData<int>>().ToArray(), _model);
        }



    }
}