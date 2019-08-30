using LottoData.Lib.Interfaces.DataTypes;

namespace LottoData.Lib.Interfaces.Features
{
    public interface ISingleRowFeature:IFeature
    {
        IData Extract(IData row);
        
    }
}