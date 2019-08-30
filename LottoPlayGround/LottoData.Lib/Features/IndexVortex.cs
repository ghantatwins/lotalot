using LottoData.Lib.DataTypes;
using LottoData.Lib.Factories;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Features;

namespace LottoData.Lib.Features
{
    public class IndexVortex : ISingleRowFeature
    {
        public string FeatureName
        {
            get
            {
                return FeatureNames.IndexSumVortex;
            }
        }
        private readonly IFeaturesFactory _featuresManager;

        public IndexVortex() : this(new FeaturesFactory())
        {

        }

        public IndexVortex(FeaturesFactory featuresManager)
        {
            _featuresManager = featuresManager;
        }

        public IData Extract(IData row)
        {
            return new FeatureData<long>(FeatureName, _featuresManager.VortexNumber(((IDraw)row).ModelIndex));
        }
    }
}