using System.Collections.Generic;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Providers.Lottery;

namespace LottoData.Lib.Interfaces
{
    public interface ILottoManager
    {
        IList<IData> Data { get; }
        ILotteryDataFactory DataFactory { get; }
        ILotterySpec Spec { get; }
        string Path { get; }
        void Export(string path);

    }
    public interface IMultiLottoManager
    {
        IList<IData> Data { get; }
        ILotteryDataFactory DataFactory { get; }
        IMultiLotterySpec Spec { get; }
        string Path { get; }
        void Export(string path);

    }
}