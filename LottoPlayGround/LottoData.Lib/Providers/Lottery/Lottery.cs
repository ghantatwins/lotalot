using System;
using Lotto.Combinatorics;
using LottoData.Lib.DataSets;
using LottoData.Lib.Interfaces.DataSets;
using LottoData.Lib.Interfaces.Providers.Lottery;

namespace LottoData.Lib.Providers.Lottery
{
    public class Lottery:ILottoConfig
    {
        
        public int TotalBalls { get; }
        public int BallsCount { get; }
        public double GameOdds { get; }
        public long TotalCombinations { get; }
        public DayOfWeek PlayedWeekDay { get; }
        public DateTime StartDay { get; }
      
        public ICombination<int> Model { get; }

        public Lottery(IMultiLotterySpec spec)
        {
         
            Model = spec.CreateModel();
            TotalBalls = Model.TotalElements;
            BallsCount = Model.ChosenElements;
            GameOdds = 1 / (double)Model.TotalCombinations;
            TotalCombinations= Model.TotalCombinations;
            StartDay = spec.BeginDay;
            PlayedWeekDay = StartDay.DayOfWeek;
           
        }
        public Lottery(ILotterySpec spec)
        {

            Model = spec.CreateModel();
            TotalBalls = Model.TotalElements;
            BallsCount = Model.ChosenElements;
            GameOdds = 1 / (double)Model.TotalCombinations;
            TotalCombinations = Model.TotalCombinations;
            StartDay = spec.BeginDay;
            PlayedWeekDay = StartDay.DayOfWeek;

        }
        public IDataFormat Format
        {
            get
            {
                return new CsvSeFormat { VariableNamesAvailable = true };
            }
        }


        
    }
}
