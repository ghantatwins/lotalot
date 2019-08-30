using System;
using Lotto.Combinatorics;
using LottoData.Lib.Interfaces.DataSets;

namespace LottoData.Lib.Interfaces.Providers.Lottery
{
    public interface ILottoConfig
    {
        /// <summary>
        /// Total Number of Balls
        /// </summary>
        int TotalBalls { get; }

        /// <summary>
        /// Number of Balls Selected
        /// </summary>
        int BallsCount { get; }

        /// <summary>
        /// Game Odds, 1/NCr
        /// </summary>
        double GameOdds { get; }

        /// <summary>
        /// Total possible combinations, Ncr
        /// </summary>
        long TotalCombinations { get; }

        /// <summary>
        /// Day of the week played, for ex: Friday
        /// </summary>
        DayOfWeek PlayedWeekDay { get; }

        /// <summary>
        /// Day on which game started
        /// </summary>
        DateTime StartDay { get; }

        /// <summary>
        /// Stars config, if not found it is null
        /// </summary>
        /// <summary>
        /// Model for Lottery
        /// </summary>
        ICombination<int> Model { get; }

        IDataFormat Format { get; }



    }

    
}