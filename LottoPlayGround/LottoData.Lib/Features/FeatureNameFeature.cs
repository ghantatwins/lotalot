using LottoData.Lib.Interfaces.Features;

namespace LottoData.Lib.Features
{
    public class FeatureNameFeature:IFeature
    {
        public FeatureNameFeature(string featureName)
        {
            FeatureName = featureName;
        }
        public string FeatureName { get; }
    }
}