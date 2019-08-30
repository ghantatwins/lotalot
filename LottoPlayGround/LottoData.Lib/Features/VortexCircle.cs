using Lotto.Combinatorics;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Factories;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Features;

namespace LottoData.Lib.Features
{
    public class VortexCircle :  ISingleRowFeature
    {
        private readonly ICombination<int> _model;

        public string FeatureName
        {
            get
            {
                return FeatureNames.VortexCircle;
            }
        }

        private IFeaturesFactory _featuresManager;
        public VortexCircle(ICombination<int> model) : this(model, new FeaturesFactory())
        {

        }


        public VortexCircle(ICombination<int> model, IFeaturesFactory featuresManager)
        {
            _featuresManager = featuresManager;
            _model = model;
        }
        public IData Extract(IData row)
        {
            return new FeatureData<long>(FeatureName, _featuresManager.VortexCircleLevel(((IDraw)row).BallsArray, _model));
        }
    }
}