using System.Collections.Generic;
using System.Linq;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Factories;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Features;

namespace LottoData.Lib.Features
{
    public class BaseNine: IOneOnOneFeature
    {
        public string FeatureName
        {
            get { return FeatureNames.Base9; }
        }
        private readonly IFeaturesFactory _featuresManager;

        public BaseNine() : this(new FeaturesFactory())
        {

        }

        public BaseNine(IFeaturesFactory featuresManager)
        {
            _featuresManager = featuresManager;
        }
        public List<IData> Extract(IData tempSet)
        {
            return _featuresManager.ToBase9(((IDraw) tempSet).OfType<BaseData<int>>().ToArray());
        }
    }
}
