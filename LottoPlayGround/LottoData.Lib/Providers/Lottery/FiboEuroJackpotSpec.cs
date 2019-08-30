using System;
using System.Collections.Generic;
using Lotto.Combinatorics;
using LottoData.Lib.Features;
using LottoData.Lib.Interfaces.Features;
using LottoData.Lib.Interfaces.Providers.Lottery;

namespace LottoData.Lib.Providers.Lottery
{
    public class FiboEuroJackpotSpec : IMultiLotterySpec
    {
        private readonly IMultiLotterySpec _spec;

        public FiboEuroJackpotSpec(IMultiLotterySpec spec)
        {
            _spec = spec;
        }

        public FiboEuroJackpotSpec() : this(new EuroJackpotSpec())
        {

        }

        public DateTime BeginDay => _spec.BeginDay;

        public int Balls => _spec.Balls;

        public int TotalBalls => _spec.TotalBalls;

        public ICombination<int> CreateModel()
        {
            return _spec.CreateModel();
        }

        public IEnumerable<IFeature> Features
        {
            get
            {
                return new List<IFeature>
                {
                    //new FeatureNameFeature(FeatureNames.IndexSumVortex),
                    //new FeatureNameFeature(FeatureNames.VortexCircle),
                    //new FeatureNameFeature(FeatureNames.Index),
                    //new FeatureNameFeature(FeatureNames.VortexDegree),
                    new FeatureNameFeature("Date"),
                    new FeatureNameFeature("Ball1"),
                    new FeatureNameFeature("Ball2"),
                    new FeatureNameFeature("Ball3"),
                    new FeatureNameFeature("Ball4"),
                    new FeatureNameFeature("Ball5"),
                    new FeatureNameFeature(FeatureNames.BallSum),
                    new FeatureNameFeature(FeatureNames.SumIndex),
                    new FeatureNameFeature(FeatureNames.SumDiff),
                    new FeatureNameFeature(FeatureNames.SumDiffFactor)
                    //new FeatureNameFeature(FeatureNames.FiboPattern),
                    //new FeatureNameFeature(FeatureNames.PrimePattern),
                    //new FeatureNameFeature(FeatureNames.PrimeCount),
                    //new FeatureNameFeature(FeatureNames.FiboCount),
                    //new FeatureNameFeature(FeatureNames.Fibo),
                    //new FeatureNameFeature(FeatureNames.FiboDiff),
                    //new FeatureNameFeature(FeatureNames.FiboDiffVortex),
                    //new FeatureNameFeature(FeatureNames.FiboBin),
                    //new FeatureNameFeature(FeatureNames.Fib2BinDec),
                    //new FeatureNameFeature(FeatureNames.FibColl),
                    //new FeatureNameFeature(FeatureNames.FiboZeros),
                    //new FeatureNameFeature(FeatureNames.FiboOnes),


                };
            }
        }

        public int StarBalls => _spec.StarBalls;

        public int StarTotalBalls => _spec.StarTotalBalls;
    }

    }