using System;
using System.Collections.Generic;
using System.Linq;
using Lotto.Combinatorics;
using LottoData.Lib.Features;
using LottoData.Lib.Interfaces.Features;
using LottoData.Lib.Interfaces.Providers.Lottery;

namespace LottoData.Lib.Providers.Lottery
{
    public class EuroJackpotSpec: IMultiLotterySpec
    {
        private MultiCombination<int> _model;


        public EuroJackpotSpec():this(5,50,2,10, new DateTime(2012, 03, 23))
        {
           
        }

        

        private EuroJackpotSpec(int balls, int totalBalls,int subBalls,int subTotalBalls, DateTime beginDay)
        {
            StarBalls = subBalls;
            StarTotalBalls = subTotalBalls;
            Balls = balls;
            TotalBalls = totalBalls;
            BeginDay = beginDay;
        }

        public DateTime BeginDay { get; }
        public int Balls { get; }
        public int TotalBalls { get; }
        public ICombination<int> CreateModel()
        {
            if (_model == null)
            {
                var total = Enumerable.Range(1, TotalBalls).ToArray();
                var subTotal = Enumerable.Range(1, StarTotalBalls).ToArray();
                _model= new MultiCombination<int>(Balls, StarBalls, total, subTotal);
            }

            return _model;
        }
        public IEnumerable<IFeature> Features
        {
            get
            {
                _model = (MultiCombination<int>) CreateModel();
                return new List<IFeature>
                {
                    //new BallSum(),
                    //new BallSumVortex(),
                    //new IndexVortex(),
                    //new PostionVortex()
                    //new BaseNine(),
                    //new VortexCircle(CreateModel()),
                    new Index(_model),
                    new SubIndex(_model.Main,SubIndex.Choice.Main),
                    new SubIndex(_model.Sub,SubIndex.Choice.Star),
                    //new Degree(CreateModel()),
                    //new OriginalDraw(),
                    //new PatternMatcher(model),
                    //new FiboIndexes(model),
                    //new BallSum(),
                    //new SumIndex(model)
                };
            }
        }


        public int StarBalls { get; }
        public int StarTotalBalls { get; }
    }
}