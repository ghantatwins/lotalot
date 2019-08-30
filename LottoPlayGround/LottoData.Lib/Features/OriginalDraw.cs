using System.Collections.Generic;
using System.Linq;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Features;

namespace LottoData.Lib.Features
{
    public class OriginalDraw : IOneOnOneFeature
    {
        public string FeatureName
        {
            get { return FeatureNames.Draw; }
        }

        public List<IData> Extract(IData tempSet)
        {
            return DrawData((IDraw)tempSet);
        }

        private List<IData> DrawData(IEnumerable<IData> balls)
        {
            return balls.ToList();
        }

        
    }
}