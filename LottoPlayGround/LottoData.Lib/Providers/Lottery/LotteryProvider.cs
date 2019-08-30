using System.Collections.Generic;
using LottoData.Lib.Interfaces.DataSets;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Providers.Lottery;

namespace LottoData.Lib.Providers.Lottery
{
    public class LotteryProvider:Provider,ILotteryProvider
    {
        private readonly ILotteryDataFactory _factory;

        public LotteryProvider(IDataFactory factory) : base(factory)
        {
            _factory = (ILotteryDataFactory) factory;
            
        }

        

       

        public IDataSet ImportDataSet(string inputFilePath)
        {
            return ImportDataSet(inputFilePath, _factory.Config);
        }

        private IDataSet ImportDataSet(string inputFilePath, ILottoConfig config)
        {
            return ImportDataSet(inputFilePath, config.Format);
        }

        public IList<IData> ImportData(string inputFilePath)
        {
            return ImportData(inputFilePath, _factory.Config);
        }

        private IList<IData> ImportData(string inputFilePath, ILottoConfig config)
        {
            return ImportData(inputFilePath, config.Format);
        }
    }
}
