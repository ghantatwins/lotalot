using System.Linq;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Factories;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Features;

namespace LottoData.Lib.Features
{
    public class BallSumVortex : ISingleRowFeature
    {
        public string FeatureName
        {
            get
            {
                return FeatureNames.BallSumVortex;
            }
        }
        private readonly IFeaturesFactory _featuresManager;

        public BallSumVortex() : this(new FeaturesFactory())
        {

        }

        public BallSumVortex(IFeaturesFactory featuresManager)
        {
            _featuresManager = featuresManager;
        }
        public IData Extract(IData row)
        {
            return new FeatureData<long>(FeatureName, _featuresManager.VortexNumber(((IDraw)row).BallsArray.Sum()));
        }
    }
}