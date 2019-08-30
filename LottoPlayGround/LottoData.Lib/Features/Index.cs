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
    public class Index :  ISingleRowFeature
    {
        private readonly ICombination<int> _model;

        public virtual string FeatureName
        {
            get
            {
                return FeatureNames.Index;
            }
        }
        private readonly IFeaturesFactory _featuresManager;

      

        public Index(ICombination<int> model,IFeaturesFactory featuresManager)
        {
            _featuresManager = featuresManager;
            _model = model;
        }
        public Index(ICombination<int> model): this(model,new FeaturesFactory())
        {
           
        }
        public virtual IData Extract(IData row)
        {
            return new FeatureData<long>(FeatureName, _featuresManager.CombIndex(((IDraw)row).BallsArray, _model));
        }

        
    }

    public class SubIndex : Index
    {
        private readonly ICombination<int> _model;
        private readonly Choice _choice;

        public enum Choice
        {
            Main,
            Star
        }

        public SubIndex(ICombination<int> model, IFeaturesFactory featuresManager) : base(model, featuresManager)
        {
          
        }

        public SubIndex(ICombination<int> model,Choice choice) : base(model)
        {
            _model = model;
            _choice = choice;
        }

        public override IData Extract(IData row)
        {
            return new FeatureData<long>(FeatureName, CombIndex(((IDraw)row).BallsArray));
        }

        private long CombIndex(int[] ballsArray)
        {
            var comparer = Comparer<int>.Default;
            if (_choice==Choice.Main)
             return (long)_model.GetIndexOf(ballsArray.Take(_model.ChosenElements).ToArray(), comparer);
            return (long)_model.GetIndexOf(ballsArray.Skip(ballsArray.Length-_model.ChosenElements).ToArray(), comparer);

        }
        public override string FeatureName
        {
            get
            {
                return FeatureNames.Index+ "-" + _choice;
            }
        }
    }
}