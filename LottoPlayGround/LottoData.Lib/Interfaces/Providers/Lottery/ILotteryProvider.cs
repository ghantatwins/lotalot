using System.Collections.Generic;
using LottoData.Lib.Interfaces.DataSets;
using LottoData.Lib.Interfaces.DataTypes;

namespace LottoData.Lib.Interfaces.Providers.Lottery
{
    public interface ILotteryProvider:IProvider
    {
        IDataSet ImportDataSet(string inputFilePath);
        IList<IData> ImportData(string inputFilePath);
    }
}
