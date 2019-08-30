using System;
using System.Collections.Generic;
using System.Linq;
using Lotto.Combinatorics;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Factories;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Providers.Lottery;
using LottoData.Lib.Providers.Lottery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lotto.Lib.Tests
{
    [TestClass]
    public class LotteryManagerTests
    {
        
        private EuroJackpotSpec _spec;
  
        private FeaturesFactory _featuresManager;
        private LotteryProvider _provider;


        private IData CreateDraw(int[] drawBalls,ICombination<int> model, DateTime drawDate)
        {
            List<IData> balls = new List<IData>
            {
                new BaseData<DateTime>(drawDate,"Date")
            };
            for (int i = 0; i < drawBalls.Length; i++)
            {
                balls.Add(new BaseData<int>(drawBalls[i], "Ball" + (i + 1)));
            }

            return new Draw(balls, model);
        }
        [TestInitialize]
        public void Setup()
        {
          
            _spec=new EuroJackpotSpec();
             var lottery = new Lottery(_spec);
             var  lotteryDataFactory=new MultiLotteryDataFactory(lottery);
           _provider = new LotteryProvider(lotteryDataFactory);
            _featuresManager=new FeaturesFactory();
            
        }

        

        [TestMethod]
        public void SaveDraws()
        {
            string outputFile = @"E:\projects\save.csv";
            ILotterySpec spec = new EuroJackpotSpec();
            var lottery = new Lottery(spec);
            var provider = new LotteryProvider(new LottoFeaturesDataFactory(new MultiLotteryDataFactory(lottery), spec.Features));
            var mainData = provider.ImportData(@"E:\projects\eurojackpot-archive-2018-oct.csv");
            provider.ExportData(mainData, outputFile);

        }

        
       

        [TestMethod]
        public void TestFiboCoding()
        {
            string expected = 0 + "" + 1010001 +"";
            string actual = _featuresManager.FibonomialNumber(30);

            Assert.AreEqual(actual,expected);
        }
        [TestMethod]
        public void TestFiboLists()
        {
          
            List<long> actual = _featuresManager.FibonomialNumbers(30).ToList();

            Assert.IsTrue(actual.Count != 0);
        }

        [TestMethod]
        public void TestFiboCoding2()
        {
            string expected = 10000000000+"";
            string actual = _featuresManager.FibonomialNumber(144);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void TestFiboLists2()
        {

            List<long> actual = _featuresManager.FibonomialNumbers(144).ToList();

            Assert.IsTrue(actual.Count != 0);
        }

        [TestMethod]
        public void TestFiboCoding3()
        {
            string expected = "0101010001010000010101001000000";
            string actual = _featuresManager.FibonomialNumber(2097472);

            Assert.AreEqual(actual, expected);
        }
        [TestMethod]
        public void TestFiboLists3()
        {

            List<long> actual = _featuresManager.FibonomialNumbers(1663).ToList();

            Assert.IsTrue(actual.Count != 0);
        }

        [TestMethod]
        public void TestFiboCodingToNumber()
        {
            long expected = 781334;
            decimal actual = _featuresManager.FibBinToDec("0-1-0-1-0-0-1-0-1-0-0-1-0-1-0-1-0-0-1-0-1-0-1-0-0-0-0-0-0");

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void TestMaxComFiboCoding()
        {
            //string expected = "0101010001010000010101001000000";
            string actual = _featuresManager.FibonomialNumber(_spec.CreateModel().TotalCombinations-1);
            Console.WriteLine(actual);
            Console.WriteLine(actual.ToCharArray().Count(x=>x=='1'));
            Console.WriteLine(actual.ToCharArray().Count(x => x == '0'));
            //Assert.AreEqual(actual, expected);
        }

        }

   }
