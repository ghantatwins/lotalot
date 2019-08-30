using LottoData.Lib.Interfaces.Providers.Lottery;

namespace LottoData.Lib.Interfaces.Factories
{
    public interface ILotteryDataFactory : IDataFactory
    {
        ILottoConfig Config { get; }
       
       
        
    }
    
}