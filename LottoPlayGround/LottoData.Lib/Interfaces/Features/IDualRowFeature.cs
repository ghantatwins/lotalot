using System.Collections.Generic;
using LottoData.Lib.Interfaces.DataTypes;

namespace LottoData.Lib.Interfaces.Features
{
    public interface IDualRowFeature:IFeature
    {
        IData Extract(List<IData> tempSet);
    }

    public interface IOneOnOneFeature : IFeature
    {
        List<IData> Extract(IData tempSet);
    }
}