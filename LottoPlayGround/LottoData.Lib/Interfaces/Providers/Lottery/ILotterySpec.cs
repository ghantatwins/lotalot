using System;
using System.Collections.Generic;
using Lotto.Combinatorics;
using LottoData.Lib.Interfaces.Features;

namespace LottoData.Lib.Interfaces.Providers.Lottery
{
    public interface ILotterySpec
    {
        DateTime BeginDay { get; }
        int Balls { get; }
        int TotalBalls { get; }
        ICombination<int> CreateModel();
        IEnumerable<IFeature> Features { get; }
    }
    public interface IMultiLotterySpec: ILotterySpec
    {
        int StarBalls { get; }
        int StarTotalBalls { get; }
    }
}